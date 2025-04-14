using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using ExcelDataReader;
using System.Text;
using Elasticsearch.Net;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage("Server=.;Database=HangfireDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
);
});

builder.Services.AddHangfireServer();
builder.Services.AddTransient<ExcelToElasticJob>();

builder.Services.AddSingleton(serviceProvider =>
{
    var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
    .BasicAuthentication("elastic", "eDF2=qg*w3skO3h1mY6v")
    .DefaultIndex("products");


    return new ElasticClient(settings);
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHangfireDashboard();

app.MapControllers();

app.MapGet("/", () => "Excel to Elastic with .NET 8, Hangfire!");

app.Run();
