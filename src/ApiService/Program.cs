using ApiService.Configuration;
using ApiService.Features.Auth.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Infrastructure;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["JWT:SigningKey"]);

#region Configuration

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JWT"));

#endregion

builder.Services.AddAuthorization();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

#region Services

builder.Services.AddScoped<AuthTokenService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen(options => { options.Path = "/openapi/{documentName}.json"; });
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.Run();