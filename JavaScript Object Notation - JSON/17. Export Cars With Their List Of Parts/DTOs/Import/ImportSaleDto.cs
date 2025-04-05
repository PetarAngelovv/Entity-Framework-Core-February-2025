using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSaleDto
    {
        [Required]
        [JsonProperty(nameof(carId))]
        public string carId { get; set; }

        [Required]
        [JsonProperty(nameof(customerId))]
        public string customerId { get; set; }

        [Required]
        [JsonProperty("discount")]
        public string Discount { get; set; }
    }
}
