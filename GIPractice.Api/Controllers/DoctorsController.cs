using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/doctors?search=&labId=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors(
        [FromQuery] string? search = null,
        [FromQuery] int? labId = null)
    {
        var query = _db.Doctors
            .Include(d => d.Lab)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(d =>
                d.FirstName.Contains(search) ||
                d.LastName.Contains(search));
        }

        if (labId.HasValue)
        {
            query = query.Where(d => d.LabId == labId.Value);
        }

        var doctors = await query
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();

        var dto = _mapper.Map<List<DoctorDto>>(doctors);
        return Ok(dto);
    }

    // GET /api/doctors/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
    {
        var doctor = await _db.Doctors
            .Include(d => d.Lab)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
            return NotFound();

        var dto = _mapper.Map<DoctorDto>(doctor);
        return Ok(dto);
    }

    // POST /api/doctors
    [HttpPost]
    public async Task<ActionResult> CreateDoctor([FromBody] DoctorCreateUpdateDto dto)
    {
        var doctor = new Doctor
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Specialty = dto.Specialty,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Score = dto.Score,
            Notes = dto.Notes ?? string.Empty,
            LabId = dto.LabId
        };

        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, null);
    }

    // PUT /api/doctors/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDoctor(int id, [FromBody] DoctorCreateUpdateDto dto)
    {
        var doctor = await _db.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        doctor.FirstName = dto.FirstName.Trim();
        doctor.LastName = dto.LastName.Trim();
        doctor.Specialty = dto.Specialty;
        doctor.Email = dto.Email;
        doctor.PhoneNumber = dto.PhoneNumber;
        doctor.Address = dto.Address;
        doctor.Score = dto.Score;
        doctor.Notes = dto.Notes ?? string.Empty;
        doctor.LabId = dto.LabId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/doctors/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDoctor(int id)
    {
        var doctor = await _db.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        // If you ever want to guard against deleting doctors that still have Tests/Treatments,
        // you can add a check here.

        _db.Doctors.Remove(doctor);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
