using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiService.Features.Auth;

public static class AuthTelemetry
{
    public static readonly ActivitySource ActivitySource = new("Mead.Auth");
    public static readonly Meter Meter = new("Mead.Auth.Meter");

    public static readonly Counter<long> LoginCounter =
        Meter.CreateCounter<long>("mead_auth_logins_total", "logins", "Number of login attempts");

    public static readonly Counter<long> LoginFailedCounter =
        Meter.CreateCounter<long>("mead_auth_login_failed_total", "logins", "Number of failed login attempts");

    public static readonly Counter<long> LogoutCounter =
        Meter.CreateCounter<long>("mead_auth_logouts_total", "logouts", "Number of logouts");

    public static readonly Counter<long> RegisterCounter = Meter.CreateCounter<long>("mead_auth_registers_total", "registers", "Number of registrations");
}