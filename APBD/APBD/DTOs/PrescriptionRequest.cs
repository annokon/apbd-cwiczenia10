namespace APBD.DTOs;

public class PrescriptionRequest
{
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public PatientDto Patient { get; set; } = null!;
    public List<MedicamentDto> Medicaments { get; set; } = new();
    public int DoctorId { get; set; }
}

public class PatientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class MedicamentDto
{
    public string Name { get; set; } = string.Empty;
    public int Dose { get; set; }
    public string Description { get; set; } = string.Empty;
}