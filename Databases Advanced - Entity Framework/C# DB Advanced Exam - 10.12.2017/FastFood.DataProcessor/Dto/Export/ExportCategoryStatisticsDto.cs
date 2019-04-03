using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    [XmlType("Category")]
    public class ExportCategoryStatisticsDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("MostPopularItem")]
        public MostPopularItemDto MostPopularItem { get; set; }
    }
    

    public class MostPopularItemDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("TotalMade")]
        public decimal TotalMade { get; set; }

        [XmlElement("TimesSold")]
        public int TimesSold { get; set; }
    }
}
