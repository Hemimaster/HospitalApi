using HospitalApi.Exceptions;
using HospitalApi.Models;
using HospitalApi.Data;
using HospitalApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PatientResponseDto>> GetPatientsAsync(string? search)
    {
        var query = _context.Patients
            .Include(p => p.Admissions)
                .ThenInclude(a => a.Ward)
            .Include(p => p.BedAssignments)
                .ThenInclude(ba => ba.Bed)
                    .ThenInclude(b => b.BedType)
            .Include(p => p.BedAssignments)
                .ThenInclude(ba => ba.Bed)
                    .ThenInclude(b => b.Room)
                        .ThenInclude(r => r.Ward)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, $"%{search}%") ||
                EF.Functions.Like(p.LastName, $"%{search}%"));
        }

        return await query
            .Select(p => new PatientResponseDto
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",

                Admissions = p.Admissions.Select(a => new AdmissionDto
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardDto
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }).ToList(),

                BedAssignments = p.BedAssignments.Select(ba => new BedAssignmentDto
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedDto
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeDto
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new RoomDto
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardDto
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                }).ToList()
            })
            .ToListAsync();
    }
    
    public async Task<int> AssignBedToPatientAsync(string pesel, CreateBedAssignmentDto dto)
    {
        if (dto.To.HasValue && dto.From >= dto.To.Value)
        {
            throw new BadRequestException("Assignment start date must be earlier than end date.");
        }

        if (string.IsNullOrWhiteSpace(dto.BedType))
        {
            throw new BadRequestException("Bed type is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Ward))
        {
            throw new BadRequestException("Ward is required.");
        }

        var patientExists = await _context.Patients.AnyAsync(p => p.Pesel == pesel);

        if (!patientExists)
        {
            throw new NotFoundException($"Patient with PESEL {pesel} was not found.");
        }

        var bedType = await _context.BedTypes
            .FirstOrDefaultAsync(bt => bt.Name == dto.BedType);

        if (bedType is null)
        {
            throw new NotFoundException($"Bed type '{dto.BedType}' was not found.");
        }

        var ward = await _context.Wards
            .FirstOrDefaultAsync(w => w.Name == dto.Ward);

        if (ward is null)
        {
            throw new NotFoundException($"Ward '{dto.Ward}' was not found.");
        }

        var availableBed = await _context.Beds
            .Where(b => b.BedTypeId == bedType.Id && b.Room.WardId == ward.Id)
            .Where(b => !b.BedAssignments.Any(ba =>
                (dto.To == null || ba.From < dto.To) &&
                (ba.To == null || dto.From < ba.To)))
            .FirstOrDefaultAsync();

        if (availableBed is null)
        {
            throw new NotFoundException(
                $"No available bed of type '{dto.BedType}' was found in ward '{dto.Ward}' for the selected period.");
        }

        var bedAssignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = dto.From,
            To = dto.To
        };

        _context.BedAssignments.Add(bedAssignment);
        await _context.SaveChangesAsync();

        return bedAssignment.Id;
    }
}