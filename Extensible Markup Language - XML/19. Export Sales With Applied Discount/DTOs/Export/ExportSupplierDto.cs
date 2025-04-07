using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("supplier")]
    public class ExportSupplierDto

    {   [XmlAttribute(nameof(id))]
        public string id { get; set; }

        [XmlAttribute(nameof(name))]
        public string name { get; set; }
    
        [XmlAttribute("parts-count")]
        public string partsCount { get; set; }

    }
}