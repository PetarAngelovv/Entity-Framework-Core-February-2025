using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductDto
    {
        [XmlElement(nameof(CategoryId))]
        public string CategoryId { get; set; }


        [XmlElement(nameof(ProductId))]
        public string ProductId { get; set; }
    }
}
