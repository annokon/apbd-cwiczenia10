using APBD.Data;
using APBD.DTOs;
using APBD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly ApplicationContext _context;

    public PrescriptionController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequest request)
    {
        if (request.Medicaments.Count > 10)
        {
            return BadRequest("Recepta może obejmować maksymalnie 10 leków.");
        }

        if (request.DueDate < request.Date)
        {
            return BadRequest("DueDate musi być większa lub równa Date.");
        }

        var patient = await _context.Patients.SingleOrDefaultAsync(
            p => p.IdPatient == request.Patient.IdPatient && p.FirstName == request.Patient.FirstName && 
                 p.LastName == request.Patient.LastName);
        if (patient == null)
        {
            patient = new Patient { IdPatient = request.Patient.IdPatient, FirstName = request.Patient.FirstName, LastName = request.Patient.LastName };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        foreach (var med in request.Medicaments)
        {
            if (!await _context.Medicaments.AnyAsync(m => m.IdMedicament == med.IdMedicament))
            {
                return NotFound($"Lek {med.IdMedicament} nie istnieje.");
            }
        }

        var doctor = await _context.Doctors.FindAsync(request.DoctorId);
        if (doctor == null)
        {
            return NotFound($"Lekarz o id {request.DoctorId} nie istnieje.");
        }

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            Patient = patient,
            Doctor = doctor
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        foreach (var med in request.Medicaments)
        {
            var medicament = await _context.Medicaments.SingleOrDefaultAsync(m => m.IdMedicament == med.IdMedicament);
            _context.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = medicament.IdMedicament,
                Dose = med.Dose,
                Details = med.Description
            });
        }

        await _context.SaveChangesAsync();
        return Ok(prescription);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
            .ThenInclude(p => p.Doctor)
            .SingleOrDefaultAsync(p => p.IdPatient == id);

        if (patient == null)
        {
            return NotFound();
        }

        var result = new
        {
            patient.IdPatient,
            patient.FirstName,
            patient.LastName,
            Prescriptions = patient.Prescriptions.Select(p => new
            {
                p.IdPrescription,
                p.Date,
                p.DueDate,
                Medicaments = p.PrescriptionMedicaments.Select(pm => new
                {
                    pm.Medicament.Name,
                    pm.Dose,
                    pm.Details
                }),
                Doctor = new
                {
                    p.Doctor.IdDoctor,
                    p.Doctor.FirstName,
                    p.Doctor.LastName
                }
            }).OrderBy(p => p.DueDate)
        };

        return Ok(result);
    }
}
