using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSupplierDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name{ get; set; }

        [Required]
        [JsonProperty(nameof(isImporter))]
        public string isImporter { get; set; }
    }
}
