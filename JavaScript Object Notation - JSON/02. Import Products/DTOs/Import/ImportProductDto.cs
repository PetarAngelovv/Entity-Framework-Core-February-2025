using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Import
{
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
