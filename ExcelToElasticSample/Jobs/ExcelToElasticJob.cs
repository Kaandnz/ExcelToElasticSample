using Nest;
using ExcelDataReader;
using ExcelToElasticSample.Models;
using ExcelToElasticSample.Services;
using Elasticsearch.Net;

namespace ExcelToElasticSample.Jobs
{
    public class ExcelToElasticJob
    {
        private readonly ElasticClient _client;

        public ExcelToElasticJob(ElasticClient client)
        {
            _client = client;
            var ping = _client.Ping();
            Console.WriteLine($"Elasticsearch ping sonucu: {ping.IsValid}  |  Hata: {ping.OriginalException?.Message}");
        }


        public void ExecuteJob(string excelFilePath)
        {
            Console.WriteLine("Job başladı…");

            // 1) Elasticsearch ping
            var ping = _client.Ping();
            Console.WriteLine($"Ping: {ping.IsValid} | Hata: {ping.OriginalException?.Message}");

            var productList = new List<Product>();

            using var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            bool isHeader = true;
            while (reader.Read())
            {
                if (isHeader) { isHeader = false; continue; }
                productList.Add(ExcelProductMapper.Map(reader));
            }

            Console.WriteLine($"Excel’den okunan satır: {productList.Count}");

            var bulkResponse = _client.Bulk(b => b
                .Index(ElasticIndexConstants.ProductIndex)
                .IndexMany(productList, (d, p) => d.Id(p.Id))
            );

            if (!bulkResponse.IsValid || bulkResponse.Errors)
            {
                Console.WriteLine("❌ BULK HATALI — detay:");
                Console.WriteLine(bulkResponse.DebugInformation);

                foreach (var item in bulkResponse.ItemsWithErrors)
                    Console.WriteLine($"ID:{item.Id} | {item.Error?.Type} – {item.Error?.Reason}");
            }
            else
            {
                Console.WriteLine($"✅ Bulk OK — eklenen belge: {bulkResponse.Items.Count}");
            }
        }

    }
}
