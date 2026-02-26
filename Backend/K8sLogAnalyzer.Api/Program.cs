using K8sLogAnalyzer.Application.Interfaces;
using K8sLogAnalyzer.Application.Services;
using K8sLogAnalyzer.Infrastructure.Kubernetes;
using K8sLogAnalyzer.Infrastructure.Parsers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "K8s Log Analyzer API", 
        Version = "v1",
        Description = "API for analyzing Kubernetes pod logs"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register application services
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IKubernetesService, KubernetesService>();
builder.Services.AddSingleton<ILogParser, LogParser>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8s Log Analyzer API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
