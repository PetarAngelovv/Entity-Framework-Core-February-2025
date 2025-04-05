using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportCustomerDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name  { get; set; }

        [Required]
        [JsonProperty(nameof(birthDate))]
        public string birthDate { get; set; }

        [Required]
        [JsonProperty(nameof(isYoungDriver))]
        public string isYoungDriver { get; set; }
    }
}
