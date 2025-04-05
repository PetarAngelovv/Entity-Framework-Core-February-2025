using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Export
{
    public class ExportCustomerDto
    {
        [Required]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }

        [Required]
        [JsonProperty(nameof(BirthDate))]
        public string BirthDate { get; set; }

        [Required]
        [JsonProperty(nameof(IsYoungDriver))]
        public string IsYoungDriver { get; set; }


    }
}
