using ProductShop.DTOs.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("SoldProducts")]
    public class ExportSoldProductsDto
    {
        [XmlElement("count")]    
        public int Count { get; set; }

        [XmlArray("products")]
        [XmlArrayItem("Product")]
        public ExportProductDto[] Products { get; set; }
    }
}
