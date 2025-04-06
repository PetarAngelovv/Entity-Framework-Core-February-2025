﻿using ProductShop.DTOs.Import;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.Utilities
{
    public class XmlHelper
    {
        public static T? Deserialize<T>(string inputXml, string rootName)
        where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);
            object? result = serializer.Deserialize(stringReader);

            return result as T;
        }

        public static T? Deserialize<T>(Stream inputXmlStream, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            object? result = serializer.Deserialize(inputXmlStream);

            return result as T;
        }

        public static string Serialize<T>(T obj, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);

            return stringWriter.ToString();
        }
    }
}
