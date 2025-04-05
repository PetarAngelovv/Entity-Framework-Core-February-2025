using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSaleDto
    {
        [JsonProperty(nameof(carId))]
        public string carId { get; set; }

        [JsonProperty(nameof(customerId))]
        public string customerId { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }
    }
}
