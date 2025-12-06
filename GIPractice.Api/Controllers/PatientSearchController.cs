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
                    Page = request.Page <= 0 ? 1 : request.Page,
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

        // Paging safety
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        if (pageSize > 200) pageSize = 200; // arbitrary upper bound

        var totalCount = await query.CountAsync();

        var patients = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
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

        var items = patients
            .Select(p => new PatientListItemDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                FathersName = p.FathersName,
                PersonalNumber = p.PersonalNumber.Value,
                BirthDay = p.BirthDay,
                AgeYears = CalcAge(p.BirthDay),
                PhoneNumber = p.PhoneNumber,
                Email = p.Email
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
