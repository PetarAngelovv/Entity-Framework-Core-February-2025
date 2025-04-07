using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("part")]
    public class ExportCarPartDto
    {
        [XmlAttribute(nameof(name))]
        public string name { get; set; }

        [XmlAttribute(nameof(price))]
        public decimal price { get; set; }
    }
}