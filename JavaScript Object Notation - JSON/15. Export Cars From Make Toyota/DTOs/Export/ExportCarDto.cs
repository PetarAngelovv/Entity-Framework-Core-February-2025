using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Export
{
    public class ExportCarDto
    {
        [JsonProperty(nameof(Id))]
        public string Id { get; set; }

        [JsonProperty(nameof(Make))]
        public string Make { get; set; }

        [JsonProperty(nameof(Model))]
        public string Model { get; set; }
     
        [JsonProperty(nameof(TraveledDistance))]
        public string TraveledDistance { get; set; }
    }
}