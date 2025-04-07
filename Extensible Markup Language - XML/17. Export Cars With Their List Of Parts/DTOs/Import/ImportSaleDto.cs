using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Sale")]
    public class ImportSaleDto
    {
        [XmlElement(nameof(carId))]
        public string carId { get; set; }


        [XmlElement(nameof(customerId))]
        public string customerId { get; set; }


        [XmlElement(nameof(discount))]
        public string discount { get; set; }

    }
}
