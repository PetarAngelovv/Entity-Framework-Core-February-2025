﻿using CarDealer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportCarDto
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("traveledDistance")]
        public string TraveledDistance { get; set; }

        [JsonProperty("partsId")]
        public List<string> PartsId { get; set; }

   
    }
}
