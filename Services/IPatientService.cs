using HospitalApi.DTOs;

namespace HospitalApi.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetPatientsAsync(string? search);
    Task<int> AssignBedToPatientAsync(string pesel, CreateBedAssignmentDto dto);
}