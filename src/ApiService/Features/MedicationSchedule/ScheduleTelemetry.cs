using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiService.Features.MedicationSchedule;

public static class ScheduleTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Mead.Schedule");
    public static readonly Meter Meter = new("Mead.Schedule.Meter");

    // Schedule operations
    public static readonly Counter<long> ScheduleCreatedCounter =
        Meter.CreateCounter<long>("mead_schedule_created_total", "schedules", "Number of medication schedules created");

    public static readonly Counter<long> ScheduleUpdatedCounter =
        Meter.CreateCounter<long>("mead_schedule_updated_total", "schedules", "Number of medication schedules updated");

    public static readonly Counter<long> ScheduleDeletedCounter =
        Meter.CreateCounter<long>("mead_schedule_deleted_total", "schedules", "Number of medication schedules deleted");

    public static readonly Counter<long> ScheduleViewedCounter =
        Meter.CreateCounter<long>("mead_schedule_viewed_total", "schedule_views", "Number of schedule detail views");

    // Business metrics
    public static readonly UpDownCounter<long> ActiveSchedulesCounter =
        Meter.CreateUpDownCounter<long>("mead_schedule_active_total", "schedules", "Total number of active schedules");

    public static readonly Histogram<double> ScheduleDurationCounter =
        Meter.CreateHistogram<double>("mead_schedule_duration_days", "days", "Duration of medication schedules in days");
}