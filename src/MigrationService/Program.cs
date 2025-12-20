using Infrastructure;
using MigrationService;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
await host.RunAsync();