using InvestYes.API.Configuration;
using InvestYes.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.AddSerilogConfiguration();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.Register(builder.Configuration);

builder.Services.AddProblemDetails(); 

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


builder.Services
    .AddApiVersioningConfiguration();

builder.Services.AddOpenTelemetryConfiguration(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddJwtAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.UseSwaggerConfiguration();

//}

app.UseExceptionHandler();

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
