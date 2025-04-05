using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportPartDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("price")]
        public string Price { get; set; }

        [Required]
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        [Required]
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }
    }
}
