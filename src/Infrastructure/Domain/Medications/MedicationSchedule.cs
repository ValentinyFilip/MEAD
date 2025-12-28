using Infrastructure.Domain.Medications.Enums;

namespace Infrastructure.Domain.Medications;

public class MedicationSchedule
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;

    public decimal AmountPerTime { get; set; } // 1 (for 1 unit per time)
    public ScheduleUnit AmountUnit { get; set; } // "pill", "teaspoon"
    public FrequencyType HowOften { get; set; }
    public List<string> Times { get; set; } = []; // ["08:00", "20:00"]
    public DateTime StartsOn { get; set; }
    public DateTime? EndsOn { get; set; }
    public bool IsActive { get; set; } = true;

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}