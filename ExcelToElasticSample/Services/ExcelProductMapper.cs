using ExcelDataReader;
using ExcelToElasticSample.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ExcelToElasticSample.Services
{
    public static class ExcelProductMapper
    {
        public static Product Map(IExcelDataReader reader)
        {
            var product = new Product
            {
                Id = reader.GetValue(0)?.ToString(),
                Name = reader.GetValue(3)?.ToString(),
                ManufacturerName = reader.GetValue(9)?.ToString(),
                Price = decimal.TryParse(reader.GetValue(19)?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var priceVal) ? priceVal : (decimal?)null,
                OldPrice = decimal.TryParse(reader.GetValue(20)?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var oldPriceVal) ? oldPriceVal : (decimal?)null,
                CreateDate = DateTime.TryParse(reader.GetValue(11)?.ToString(), out var createDt) ? createDt : (DateTime?)null,
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
                IsEbook = reader.GetValue(35)?.ToString() == "1",     // düzeltildi
                IsShippingFreeProduct = reader.GetValue(60)?.ToString() == "1",     // düzeltildi
                WillBeInStock = reader.GetValue(34)?.ToString() == "1",     // düzeltildi
                Keywords = reader.GetValue(25)?.ToString(),
            };

            // -------- JSON alanları --------

            // 1) KATEGORİLER  (column 38)
            var categoryJson = reader.GetValue(38)?.ToString();   
            if (!string.IsNullOrWhiteSpace(categoryJson) && categoryJson.Trim().StartsWith("{"))
            {
                try
                {
                    var jArray = JObject.Parse(categoryJson)["category"] as JArray;
                    product.CategoryLevel1 = jArray?.ElementAtOrDefault(0)?["name"]?.ToString();
                    product.CategoryLevel2 = jArray?.ElementAtOrDefault(1)?["name"]?.ToString();
                    product.CategoryLevel3 = jArray?.ElementAtOrDefault(2)?["name"]?.ToString();
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"Kategori JSON Hatası: {ex.Message}");
                }
            }

            // 2) ATTRIBUTES  (column 39 – İlk Baskı Yılı)
            var attributeJson = reader.GetValue(39)?.ToString();  // ← düzeltildi
            if (!string.IsNullOrWhiteSpace(attributeJson) && attributeJson.Trim().StartsWith("{"))
            {
                try
                {
                    var attrs = JObject.Parse(attributeJson)["attributes"] as JArray;
                    var firstEdition = attrs?.FirstOrDefault(a => a["Text"]?.ToString() == "İlk Baskı Yılı");
                    product.FirstEditionYear = firstEdition?["Option"]?["ValueText"]?.ToString();
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"Attribute JSON Hatası: {ex.Message}");
                }
            }

            // 3) CAMPAIGN NAMES  (column 50)
            var campaignJson = reader.GetValue(50)?.ToString();   // ← düzeltildi
            if (!string.IsNullOrWhiteSpace(campaignJson) && campaignJson.Trim().StartsWith("{"))
            {
                try
                {
                    var root = JObject.Parse(campaignJson);
                    var campaignArray = root["productCampaign"] as JArray;
                    if (campaignArray != null)
                    {
                        product.CampaignNames = campaignArray
                            .Select(c => c["Name"]?.ToString())
                            .Where(n => !string.IsNullOrWhiteSpace(n))
                            .ToList();
                    }
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"productCampaign JSON hatası: {ex.Message}");
                }
            }

            return product;
        }
    }
}
