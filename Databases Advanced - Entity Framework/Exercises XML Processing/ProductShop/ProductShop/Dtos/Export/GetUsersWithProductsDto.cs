namespace ProductShop.Dtos.Export
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class UsersAndProductsDto
    {
        public UsersAndProductsDto()
        {
            this.Users = new List<UserDto>();
        }

        [XmlElement("count")]
        public int Count { get; set; }


        [XmlArray("users")]
        public List<UserDto> Users { get; set; }
    }

    [XmlType("User")]
    public class UserDto
    {

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }
        
        public SoldProductsDto SoldProducts { get; set; }
    }

    [XmlType("SoldProducts")]
    public class SoldProductsDto
    {
        public SoldProductsDto()
        {
            this.Products = new List<ProductsSoldDto>();
        }

        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public List<ProductsSoldDto> Products { get; set; }
    }
}
