using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using ExcelDataReader;
using System.Text;
using Elasticsearch.Net;
using ExcelToElasticSample.Jobs;
using ExcelToElasticSample.Mappings;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Hangfire ayarlarý
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage("Server=.;Database=HangfireDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
});
builder.Services.AddHangfireServer();

// DI servisleri
builder.Services.AddTransient<ExcelToElasticJob>();
builder.Services.AddTransient<ScheduledImportJob>();
builder.Services.AddSingleton<ProductMappingCreator>();

// Elasticsearch ayarlarý
builder.Services.AddSingleton(serviceProvider =>
{
    var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
        .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
        .BasicAuthentication("elastic", "eDF2=qg*w3skO3h1mY6v")
        .DefaultIndex("products_degistirilmis"); // Varsayýlan index

    return new ElasticClient(settings);


});

builder.Services.AddControllers();

var app = builder.Build();

// Elasticsearch mapping kontrolü
using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<ProductMappingCreator>();
    mapper.CreateMappingIfNotExists();
}

// Hangfire dashboard ve job
app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ScheduledImportJob>(
    "excel-import-recurring-job",
    job => job.Execute(),
    Cron.Minutely
);

app.MapControllers();
app.MapGet("/", () => "Excel to Elastic with .NET 8, Hangfire!");

app.Run();
