using api.Application.Interfaces;
using api.Application.Services;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevents infinite loops.. For now handy but need to see if we should want this. 
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:4173",
                "https://localhost:4173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection Registration
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IScanService, ScanService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMunicipalityService, MunicipalityService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IDetectedProductService, DetectedProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ClientDev");

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
