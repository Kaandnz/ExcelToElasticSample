using Nest;
using ExcelToElasticSample.Models;
using ExcelToElasticSample.Services;

namespace ExcelToElasticSample.Mappings
{
    public class ProductMappingCreator
    {
        private readonly ElasticClient _client;

        public ProductMappingCreator(ElasticClient client)
        {
            _client = client;
        }

        public void CreateMappingIfNotExists()
        {
            var exists = _client.Indices.Exists(ElasticIndexConstants.ProductIndex);
            if (!exists.Exists)
            {
                var createIndexResponse = _client.Indices.Create(ElasticIndexConstants.ProductIndex, c => c
                    .Map<Product>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Keyword(k => k.Name(p => p.Id))
                            .Text(t => t.Name(p => p.Name).Analyzer("standard"))
                            .Text(t => t.Name(p => p.ManufacturerName))
                            .Keyword(k => k.Name(p => p.Language))
                            .Keyword(k => k.Name(p => p.MediaType))
                            .Keyword(k => k.Name(p => p.TopCategoryName))
                            .Keyword(k => k.Name(p => p.PersonName))
                            .Keyword(k => k.Name(p => p.PublisherName))
                            .Keyword(k => k.Name(p => p.CategoryLevel1))
                            .Keyword(k => k.Name(p => p.CategoryLevel2))
                            .Keyword(k => k.Name(p => p.CategoryLevel3))
                            .Text(t => t.Name(p => p.Keywords))
                            .Keyword(k => k.Name(p => p.CampaignNames))
                            .Keyword(k => k.Name(p => p.FirstEditionYear))
                            .Number(n => n.Name(p => p.Price).Type(NumberType.Double))
                            .Number(n => n.Name(p => p.OldPrice).Type(NumberType.Double))
                            .Number(n => n.Name(p => p.ProductScore).Type(NumberType.Float))
                            .Number(n => n.Name(p => p.Rating).Type(NumberType.Float))
                            .Number(n => n.Name(p => p.StockQuantity).Type(NumberType.Integer))
                            .Number(n => n.Name(p => p.SoldCount).Type(NumberType.Integer))
                            .Number(n => n.Name(p => p.CommentCount).Type(NumberType.Integer))
                            .Number(n => n.Name(p => p.ProductFavCount).Type(NumberType.Integer))
                            .Number(n => n.Name(p => p.ProductPictureCount).Type(NumberType.Integer))
                            .Boolean(b => b.Name(p => p.IsEbook))
                            .Boolean(b => b.Name(p => p.IsShippingFreeProduct))
                            .Boolean(b => b.Name(p => p.WillBeInStock))
                            .Date(d => d.Name(p => p.CreateDate))
                            .Date(d => d.Name(p => p.PublishedOnUtc))
                        )
                    )
                );

                if (!createIndexResponse.IsValid)
                {
                    Console.WriteLine($"Mapping oluşturulurken hata: {createIndexResponse.DebugInformation}");
                }
                else
                {
                    Console.WriteLine("Mapping başarıyla oluşturuldu.");
                }
            }
        }
    }
}
