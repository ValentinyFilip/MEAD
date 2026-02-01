using BlazorFrontend.Common;
using BlazorFrontend.Components;
using BlazorFrontend.Services;
using MudBlazor;
using MudBlazor.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Local storage + auth
builder.Services.AddScoped<IAuthState, BrowserAuthState>();
builder.Services.AddScoped<AuthenticatedHttpMessageHandler>();

builder.Services.AddScoped<MedicationService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AuthenticatedHttpMessageHandler>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.ConfigureHttpClientDefaults(http => { http.AddServiceDiscovery(); });

builder.Services.AddHttpClient("AuthApi", client => { client.BaseAddress = new Uri("https+http://api-service"); });

builder.Services.AddHttpClient("BackendApi", client => { client.BaseAddress = new Uri("https+http://api-service"); })
    .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();