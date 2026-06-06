namespace HospitalApi.DTOs;

public class PatientResponseDto
{
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public ICollection<AdmissionDto> Admissions { get; set; } = new List<AdmissionDto>();
    public ICollection<BedAssignmentDto> BedAssignments { get; set; } = new List<BedAssignmentDto>();
}

public class AdmissionDto
{
    public int Id { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public WardDto Ward { get; set; } = null!;
}

public class BedAssignmentDto
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public BedDto Bed { get; set; } = null!;
}

public class BedDto
{
    public int Id { get; set; }
    public BedTypeDto BedType { get; set; } = null!;
    public RoomDto Room { get; set; } = null!;
}

public class BedTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class RoomDto
{
    public string Id { get; set; } = null!;
    public bool HasTv { get; set; }
    public WardDto Ward { get; set; } = null!;
}

public class WardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}