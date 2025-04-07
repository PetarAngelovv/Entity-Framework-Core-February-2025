using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("car")]
    public class ExportCarAttributesDto
    {
        [XmlAttribute(nameof(id))]
        public string id { get; set; }

        [XmlAttribute(nameof(model))]
        public string model { get; set; }

        [XmlAttribute("traveled-distance")]
        public string traveledDistance { get; set; }
    }
}
