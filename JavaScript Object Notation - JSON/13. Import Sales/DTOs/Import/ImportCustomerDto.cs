using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportCustomerDto
    {
        [JsonProperty("name")]
        public string Name  { get; set; }

        [JsonProperty(nameof(birthDate))]
        public string birthDate { get; set; }

        [JsonProperty(nameof(isYoungDriver))]
        public string isYoungDriver { get; set; }
    }
}
