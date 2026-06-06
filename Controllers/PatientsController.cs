using HospitalApi.DTOs;
using HospitalApi.Exceptions;
using HospitalApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalApi.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search)
    {
        var patients = await _patientService.GetPatientsAsync(search);
        return Ok(patients);
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBedToPatient(string pesel, [FromBody] CreateBedAssignmentDto dto)
    {
        try
        {
            var assignmentId = await _patientService.AssignBedToPatientAsync(pesel, dto);

            return Created($"/api/patients/{pesel}/bedassignments/{assignmentId}", new
            {
                id = assignmentId
            });
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}