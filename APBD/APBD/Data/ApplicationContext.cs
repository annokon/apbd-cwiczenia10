using APBD.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Patient>().HasData(new List<Patient>()
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Kowalski"},
            new() { Id = 2, FirstName = "Anna", LastName = "Nowak"}
        });

        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>()
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Lekarz"},
            new() { Id = 2, FirstName = "Anna", LastName = "Lekarka"}
        });

        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>()
        {
            new() { Id = 1, Name = "Paracetamol"},
            new() { Id = 2, Name = "Ibuprofen"}
        });
    }
}