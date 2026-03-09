using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using LoginNet.Infrastructure.Persistence;
using LoginNet.WebApi.Endpoints;
using LoginNet.Infrastructure.Middleware;
using LoginNet.Domain;
using LoginNet.Domain.Entities;
using LoginNet.Application;
using LoginNet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        }
    );
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure JSON serialization options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowTrailingCommas = true;
});

// Register Layer Services
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Roles.Any())
    {
        db.Roles.Add(new Role
        {
            Id = 1,
            Name = "admin",
            CanRegisterUsers = true,
            CanCreateRoles = true,
            OwnerId = null
        });
        db.SaveChanges();
    }
}

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapNoteEndpoints().RequireAuthorization();
app.MapRoleEndpoints().RequireAuthorization();

app.Run();
