using Infrastructure.Domain.Medications.Enums;

namespace Infrastructure.Domain.Medications;

public class Stock
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;

    public decimal HowManyLeft { get; set; }
    public StockUnit Unit { get; set; } // "pills", "bottles"
    public DateTime? ExpiresOn { get; set; }
    public string? BatchNumber { get; set; }
    public string? WhereItsStored { get; set; } // "bathroom cabinet"
    public decimal WarnWhenBelow { get; set; } // reorder point

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}