﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [XmlAttribute(nameof(id))]
        public string id { get; set; }
    
    }
}
