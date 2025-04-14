namespace ExcelToElasticSample.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? ManufacturerName { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public double? ProductScore { get; set; }
        public double? Rating { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime? PublishedOnUtc { get; set; }
        public string? TopCategoryName { get; set; }
        public string? Language { get; set; }
        public int? SoldCount { get; set; }
        public string? MediaType { get; set; }
        public int? CommentCount { get; set; }
        public string? PersonName { get; set; }
        public string? PublisherName { get; set; }
        public int? ProductFavCount { get; set; }
        public int? ProductPictureCount { get; set; }
        public bool? IsEbook { get; set; }
        public bool? IsShippingFreeProduct { get; set; }
    }
}
