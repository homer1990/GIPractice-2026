using GIPractice.Api.Models;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/search")]
public class PatientSearchController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;
    static int CalcAge(DateTime birthDay)
    {
        var today = DateTime.UtcNow.Date;
        var b = birthDay.Date;
        var age = today.Year - b.Year;
        if (b > today.AddYears(-age)) age--;
        return age < 0 ? 0 : age;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PatientListItemDto>>> Search(
        [FromQuery] PatientSearchRequestDto request)
    {
        var query = _db.Patients.AsNoTracking().AsQueryable();

        // PersonalNumber — exact match, if valid
        if (!string.IsNullOrWhiteSpace(request.PersonalNumber))
        {
            var raw = request.PersonalNumber.Trim();

            if (PersonalNumber.TryCreate(raw, out var pn))
            {
                query = query.Where(p => p.PersonalNumber == pn);
            }
            else
            {
                // If it's not a valid 12-digit number, we can just return empty result.
                return Ok(new PagedResultDto<PatientListItemDto>
                {
                    Page = request.PageIndex <= 0 ? 1 : request.PageIndex,
                    PageSize = request.PageSize <= 0 ? 20 : request.PageSize,
                    TotalCount = 0,
                    Items = []
                });
            }
        }

        // LastName / FirstName / FathersName - contains
        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            var last = request.LastName.Trim();
            query = query.Where(p => p.LastName.Contains(last));
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            var first = request.FirstName.Trim();
            query = query.Where(p => p.FirstName.Contains(first));
        }

        if (!string.IsNullOrWhiteSpace(request.FathersName))
        {
            var father = request.FathersName.Trim();
            query = query.Where(p => p.FathersName.Contains(father));
        }

        // Birth date range (if provided)
        if (request.BirthDateFrom.HasValue)
        {
            var from = request.BirthDateFrom.Value.Date;
            query = query.Where(p => p.BirthDay >= from);
        }

        if (request.BirthDateTo.HasValue)
        {
            var to = request.BirthDateTo.Value.Date;
            query = query.Where(p => p.BirthDay <= to);
        }

        // Phone / Email - contains
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phone = request.PhoneNumber.Trim();
            query = query.Where(p => p.PhoneNumber != null && p.PhoneNumber.Contains(phone));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            query = query.Where(p => p.Email != null && p.Email.Contains(email));
        }

        var page = request.PageIndex <= 0 ? 1 : request.PageIndex;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        if (pageSize > 200) pageSize = 200;

        var totalCount = await query.CountAsync();

        // Project patients with LastVisitUtc (for sorting + display)
        var projected = query
            .Select(p => new
            {
                Patient = p,
                LastVisitUtc = _db.Visits
                    .Where(v => v.PatientId == p.Id)
                    .OrderByDescending(v => v.DateOfVisitUtc)
                    .Select(v => (DateTime?)v.DateOfVisitUtc)
                    .FirstOrDefault()
            });

        // Normalize sort params
        var sortField = (request.SortField ?? "LastVisit").Trim();
        var sortDescending = request.SortDescending;
        var key = sortField.ToLowerInvariant();

        var ordered = key switch
        {
            "lastname" => sortDescending
                ? projected
                    .OrderByDescending(x => x.Patient.LastName)
                    .ThenByDescending(x => x.Patient.FirstName)
                : projected
                    .OrderBy(x => x.Patient.LastName)
                    .ThenBy(x => x.Patient.FirstName),

            "firstname" => sortDescending
                ? projected
                    .OrderByDescending(x => x.Patient.FirstName)
                    .ThenByDescending(x => x.Patient.LastName)
                : projected
                    .OrderBy(x => x.Patient.FirstName)
                    .ThenBy(x => x.Patient.LastName),

            "birthday" => sortDescending
                ? projected.OrderByDescending(x => x.Patient.BirthDay)
                : projected.OrderBy(x => x.Patient.BirthDay),

            "personalnumber" => sortDescending
                ? projected.OrderByDescending(x => x.Patient.PersonalNumber.Value)
                : projected.OrderBy(x => x.Patient.PersonalNumber.Value),

            "id" => sortDescending
                ? projected.OrderByDescending(x => x.Patient.Id)
                : projected.OrderBy(x => x.Patient.Id),

            // explicit "lastvisit"
            "lastvisit" => sortDescending
                ? projected.OrderByDescending(x => x.LastVisitUtc ?? DateTime.MinValue)
                : projected.OrderBy(x => x.LastVisitUtc ?? DateTime.MaxValue),

            // fallback for any unknown sort field
            _ => sortDescending
                ? projected.OrderByDescending(x => x.LastVisitUtc ?? DateTime.MinValue)
                : projected.OrderBy(x => x.LastVisitUtc ?? DateTime.MaxValue)
        };

        var pageItems = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        static int CalcAge(DateTime birthDay)
        {
            var today = DateTime.UtcNow.Date;
            var b = birthDay.Date;
            var age = today.Year - b.Year;
            if (b > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }

        var items = pageItems
            .Select(x => new PatientListItemDto
            {
                Id = x.Patient.Id,
                FirstName = x.Patient.FirstName,
                LastName = x.Patient.LastName,
                FathersName = x.Patient.FathersName,
                PersonalNumber = x.Patient.PersonalNumber.Value,
                BirthDay = x.Patient.BirthDay,
                AgeYears = CalcAge(x.Patient.BirthDay),
                PhoneNumber = x.Patient.PhoneNumber,
                Email = x.Patient.Email,
                LastVisitUtc = x.LastVisitUtc
            })
            .ToList();

        var result = new PagedResultDto<PatientListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
        return Ok(result);

    }
}
