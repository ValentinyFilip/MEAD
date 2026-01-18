using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiService.Features.Medication;

public static class MedicationsTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Mead.Medication");
    public static readonly Meter Meter = new("Mead.Medication.Meter");

    // Medication operations
    public static readonly Counter<long> MedicationCreatedCounter =
        Meter.CreateCounter<long>("mead_medications_created_total", "medications", "Number of medications created");

    public static readonly Counter<long> MedicationUpdatedCounter =
        Meter.CreateCounter<long>("mead_medications_updated_total", "medications", "Number of medications updated");

    public static readonly Counter<long> MedicationDeletedCounter =
        Meter.CreateCounter<long>("mead_medications_deleted_total", "medications", "Number of medications deleted");

    public static readonly Counter<long> MedicationMarkedUnusedCounter =
        Meter.CreateCounter<long>("mead_medications_marked_unused_total", "medications", "Number of medications marked as unused");

    public static readonly Counter<long> MedicationReactivatedCounter =
        Meter.CreateCounter<long>("mead_medications_reactivated_total", "medications", "Number of medications reactivated");

    // Medication business metrics
    public static readonly UpDownCounter<long> TotalActiveMedicationsCounter =
        Meter.CreateUpDownCounter<long>("mead_medications_total_active", "medications", "Total number of active medications");

    public static readonly Histogram<double> MedicationStrengthHistogram =
        Meter.CreateHistogram<double>("mead_medications_strength", "mg", "Distribution of medication strengths");

    // Medication queries
    public static readonly Counter<long> MedicationQueriedCounter =
        Meter.CreateCounter<long>("mead_medications_queried_total", "queries", "Number of medication detail queries");

    public static readonly Counter<long> MedicationListQueriedCounter =
        Meter.CreateCounter<long>("mead_medications_list_queried_total", "queries", "Number of medication list queries");
}