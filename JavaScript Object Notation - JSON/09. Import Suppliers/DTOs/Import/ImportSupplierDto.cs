﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSupplierDto
    {
        [JsonProperty("name")]
        public string Name{ get; set; }

        [JsonProperty(nameof(isImporter))]
        public string isImporter { get; set; }
    }
}
