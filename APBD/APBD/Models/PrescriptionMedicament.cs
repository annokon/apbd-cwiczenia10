using System.ComponentModel.DataAnnotations.Schema;

namespace APBD.Models;

[Table("prescription_medicaments")]
public class PrescriptionMedicament
{
    public int PrescriptionId { get; set; }
    [ForeignKey(nameof(PrescriptionId))]
    public Prescription Prescription { get; set; }
        
    public int MedicamentId { get; set; }
    [ForeignKey(nameof(MedicamentId))]
    public Medicament Medicament { get; set; }
        
    public int Dose { get; set; }
    public string Description { get; set; } = string.Empty;
}