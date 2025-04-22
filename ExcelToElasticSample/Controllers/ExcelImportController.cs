using ExcelToElasticSample.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ExcelImportController : ControllerBase
{
    [HttpPost]
    public IActionResult ImportExcel()
    {
        var excelFilePath = "C:\\Users\\Acer\\Downloads\\Productset.xlsx";

        BackgroundJob.Enqueue<ExcelToElasticJob>(job => job.ExecuteJob(excelFilePath));

        return Ok("Excel verisi import job'ı Hangfire kuyruğuna eklendi.");
    }
}
