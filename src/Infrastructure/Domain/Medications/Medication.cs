using Infrastructure.Domain.Medications.Enums;

namespace Infrastructure.Domain.Medications;

public class Medication
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid UserId { get; set; }
    public User.User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty; // "Panadol", "Ibuprofen"
    public string? AlsoKnownAs { get; set; } // "Paracetamol", "generic name"
    public DosageForm Form { get; set; } // Pill, Liquid...
    public decimal Strength { get; set; } // 500 (for 500mg)
    public StrengthUnit StrengthUnit { get; set; } // "mg", "ml"
    public Route HowToTake { get; set; } // ByMouth, OnSkin...
    public string? ComesFrom { get; set; } // "pharmacy", "manufacturer"
    public bool IsActive { get; set; } = true;

    // Navigation
    public List<MedicationSchedule> Schedules { get; set; } = [];
    public List<Stock> Stocks { get; set; } = [];

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsUnused { get; set; } = false;
}