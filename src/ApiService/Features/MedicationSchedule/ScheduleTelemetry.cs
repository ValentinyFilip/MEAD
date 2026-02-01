using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiService.Features.MedicationSchedule;

public static class SchedulesTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Mead.Schedules");
    public static readonly Meter Meter = new("Mead.Schedules.Meter");

    // Schedule operations
    public static readonly Counter<long> ScheduleCreatedCounter =
        Meter.CreateCounter<long>("mead_schedules_created_total", "schedules", "Number of schedules created");

    public static readonly Counter<long> ScheduleUpdatedCounter =
        Meter.CreateCounter<long>("mead_schedules_updated_total", "schedules", "Number of schedules updated");

    public static readonly Counter<long> ScheduleDeletedCounter =
        Meter.CreateCounter<long>("mead_schedules_deleted_total", "schedules", "Number of schedules deleted");

    public static readonly Counter<long> ScheduleDeactivatedCounter =
        Meter.CreateCounter<long>("mead_schedules_deactivated_total", "schedules", "Number of schedules deactivated");

    public static readonly Counter<long> ScheduleReactivatedCounter =
        Meter.CreateCounter<long>("mead_schedules_reactivated_total", "schedules", "Number of schedules reactivated");

    // Schedule business metrics
    public static readonly UpDownCounter<long> TotalActiveSchedulesCounter =
        Meter.CreateUpDownCounter<long>("mead_schedules_total_active", "schedules", "Total number of active schedules");

    public static readonly Histogram<double> ScheduleDurationDaysHistogram =
        Meter.CreateHistogram<double>("mead_schedules_duration_days", "days", "Distribution of schedule durations");

    public static readonly Histogram<long> DailyDosesHistogram =
        Meter.CreateHistogram<long>("mead_schedules_daily_doses", "doses", "Distribution of daily dose frequencies");

    // Schedule queries
    public static readonly Counter<long> ScheduleQueriedCounter =
        Meter.CreateCounter<long>("mead_schedules_queried_total", "queries", "Number of schedule detail queries");

    public static readonly Counter<long> ScheduleListQueriedCounter =
        Meter.CreateCounter<long>("mead_schedules_list_queried_total", "queries", "Number of schedule list queries");
}