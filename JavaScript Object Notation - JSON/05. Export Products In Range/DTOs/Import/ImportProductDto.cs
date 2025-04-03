
namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;


    public class ImportProductDto
    {
        [Required]
        [JsonPropertyName(nameof(Name))]
        public string Name { get; set; } = null!;

        [Required]
        [JsonPropertyName(nameof(Price))]
        public string Price { get; set; } = null!;

        [Required]
        [JsonPropertyName(nameof(SellerId))]
        public string SellerId { get; set; } = null!;


        [JsonPropertyName(nameof(BuyerId))]
        public string? BuyerId { get; set; }
    }
}
