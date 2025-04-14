using Nest;
using ExcelDataReader;
using ExcelToElasticSample.Models;

public class ExcelToElasticJob
{
    private readonly ElasticClient _client;

    public ExcelToElasticJob(ElasticClient client)
    {
        _client = client;
    }

    public void ExecuteJob(string excelFilePath)
    {
       
        var filePath = @"C:\Users\Acer\Desktop\Product.xlsx";

       
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                bool isFirstRow = true;
                var productList = new List<Product>();

                while (reader.Read())
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue; 
                    }

                    var product = new Product
                    {
                        Id = reader.GetValue(0)?.ToString(),
                        Name = reader.GetValue(3)?.ToString(),
                        ManufacturerName = reader.GetValue(9)?.ToString(),
                        CreateDate = DateTime.TryParse(reader.GetValue(11)?.ToString(), out var createDt) ? createDt : (DateTime?)null,
                        Price = decimal.TryParse(reader.GetValue(19)?.ToString(), out var priceVal) ? priceVal : (decimal?)null,
                        OldPrice = decimal.TryParse(reader.GetValue(20)?.ToString(), out var oldPriceVal) ? oldPriceVal : (decimal?)null,
                        ProductScore = double.TryParse(reader.GetValue(13)?.ToString(), out var prodScore) ? prodScore : (double?)null,
                        Rating = double.TryParse(reader.GetValue(44)?.ToString(), out var ratingVal) ? ratingVal : (double?)null,
                        StockQuantity = int.TryParse(reader.GetValue(47)?.ToString(), out var stockQty) ? stockQty : (int?)null,
                        PublishedOnUtc = DateTime.TryParse(reader.GetValue(62)?.ToString(), out var pubDate) ? pubDate : (DateTime?)null,
                        TopCategoryName = reader.GetValue(29)?.ToString(),
                        Language = reader.GetValue(16)?.ToString(),
                        SoldCount = int.TryParse(reader.GetValue(21)?.ToString(), out var soldCount) ? soldCount : (int?)null,
                        MediaType = reader.GetValue(22)?.ToString(),
                        CommentCount = int.TryParse(reader.GetValue(26)?.ToString(), out var commentCount) ? commentCount : (int?)null,
                        PersonName = reader.GetValue(30)?.ToString(),
                        PublisherName = reader.GetValue(31)?.ToString(),
                        ProductFavCount = int.TryParse(reader.GetValue(53)?.ToString(), out var favCount) ? favCount : (int?)null,
                        ProductPictureCount = int.TryParse(reader.GetValue(54)?.ToString(), out var picCount) ? picCount : (int?)null,
                        IsEbook = reader.GetValue(34)?.ToString() == "1",
                        IsShippingFreeProduct = reader.GetValue(59)?.ToString() == "1",
                    };


                    productList.Add(product);
                }

                var bulkResponse = _client.Bulk(b => b
                    .Index("products")
                    .IndexMany(productList)
                );

                Console.WriteLine("Excel verileri başarıyla Elasticsearch'e aktarıldı.");

            }
        }
    }

}
