using AspNetCoreWithEFCore7;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
var builder = WebApplication.CreateBuilder(args);

// получаем строку подключения из файла конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Log.Logger = new LoggerConfiguration()
//     .WriteTo.PostgreSQL(connectionString, "logs")
//     .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting web application");
builder.Host.UseSerilog(); // <-- Add this line

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} {RequestBody} {StatusCode}";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestBody", httpContext.Request.Body);
    };
});


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();