using CarDealer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportCarDto
    {
        [Required]
        [JsonProperty("make")]
        public string Make { get; set; }

        [Required]
        [JsonProperty("model")]
        public string Model { get; set; }

        [Required]
        [JsonProperty("traveledDistance")]
        public string TraveledDistance { get; set; }

        [JsonProperty("partsId")]
        public ICollection<int>? PartsId { get; set; }
    }
}
