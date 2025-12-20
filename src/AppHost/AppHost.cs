using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("mead-postgres")
    .WithDataVolume("mead-postgres-data");

var meadDb = postgres.AddDatabase("mead-db");

var migrationService = builder.AddProject<MigrationService>("migration-service")
    .WithReference(meadDb);

var apiService = builder.AddProject<ApiService>("api-service")
    .WithReference(meadDb)
    .WaitForCompletion(migrationService);

var frontend = builder.AddProject<BlazorFrontend>("frontend")
    .WithReference(apiService);


builder.Build().Run();