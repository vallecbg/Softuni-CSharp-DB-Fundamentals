using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using CarDealer.Models;

namespace CarDealer.Dtos.Export
{
    [XmlType("car")]
    public class GetCarsWithTheirListOfPartsDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public GetPartsDto[] Parts { get; set; }
    }

    [XmlType("part")]
    public class GetPartsDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
