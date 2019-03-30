using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ProductShop.Models;

namespace ProductShop.Dtos.Export
{
    [XmlType("Product")]
    public class GetProductsInRangeDto
    {
        //<Product>
        //<name>Parsley</name>
        //<price>519.06</price>
        //<buyer>Brendin Predohl</buyer>
        //</Product>

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("buyer")]
        public string BuyerName { get; set; }
    }
}
