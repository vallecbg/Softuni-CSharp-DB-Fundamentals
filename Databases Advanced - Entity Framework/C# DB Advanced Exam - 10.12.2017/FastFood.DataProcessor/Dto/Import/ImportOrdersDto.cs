using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using FastFood.Models;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Order")]
    public class ImportOrdersDto
    {
        [XmlElement("Customer")]
        public string Customer { get; set; }

        [XmlElement("Employee")]
        public string Employee { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlArray("Items")]
        public ItemDto[] Items { get; set; }
    }

    [XmlType("Item")]
    public class ItemDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Quantity")]
        public int Quantity { get; set; }
    }
}
