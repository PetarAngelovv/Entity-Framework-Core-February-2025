using CarDealer.Models;
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
    [XmlType("Sale")]
    public class ExportSaleDiscountDto
    {
        [XmlElement(nameof(car))]
        public ExportCarDto[] car { get; set; }


        [XmlElement(nameof(discount))]
        public int discount { get; set; }

        [XmlElement("customer-name")]
        public string customerName { get; set; }

        [XmlElement(nameof(price))]
        public decimal price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal priceWithDiscount { get; set; }
       

    }
}
