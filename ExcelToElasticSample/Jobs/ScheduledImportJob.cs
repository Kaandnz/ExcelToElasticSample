namespace ExcelToElasticSample.Jobs
{
    public class ScheduledImportJob
    {
        public async Task Execute()
        {
            using var client = new HttpClient();

            var response = await client.PostAsync("https://localhost:7216/ExcelImport", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Scheduled import basariyla tetiklendi.");
            }
            else
            {
                Console.WriteLine($"Scheduled import hatali: {response.StatusCode}");
            }
        }
    }
}
