using HospitalApi.DTOs;

namespace HospitalApi.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetPatientsAsync(string? search);
}