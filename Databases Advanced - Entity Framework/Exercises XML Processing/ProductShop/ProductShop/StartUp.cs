using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });

            //var usersXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            using (ProductShopContext context = new ProductShopContext())
            {
                var result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var productsFromXml = doc.Root
                .Elements()
                .ToList();

            var products = new List<Product>();

            productsFromXml.ForEach(x =>
            {
                Product currentProduct = new Product();
                currentProduct.Name = x.Element("name").Value;
                currentProduct.Price = Convert.ToDecimal(x.Element("price").Value);

                var sellerId = Convert.ToInt32(x.Element("sellerId").Value);
                var buyerId = Convert.ToInt32(x.Element("sellerId").Value);

                currentProduct.SellerId = sellerId;
                currentProduct.BuyerId = buyerId == 0 ? null : (int?)buyerId;
                
                products.Add(currentProduct);
            });

            context.Products.AddRange(products);

            int affectedRows = context.SaveChanges();
            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var categoriesFromXml = doc.Root
                .Elements()
                .ToList();

            var categories = new List<Category>();

            categoriesFromXml.ForEach(x =>
            {
                Category currCategory = new Category();
                currCategory.Name = x.Element("name").Value;
                if (currCategory.Name != null ||
                    currCategory.Name != "")
                {
                    categories.Add(currCategory);
                }
            });

            context.Categories.AddRange(categories);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var categoriesProductsXml = doc.Root
                .Elements()
                .ToList();

            var categoriesProducts = new List<CategoryProduct>();

            categoriesProductsXml.ForEach(x =>
            {
                var currCategoryProduct = new CategoryProduct();

                currCategoryProduct.CategoryId = int.Parse(x.Element("CategoryId").Value);
                currCategoryProduct.ProductId = int.Parse(x.Element("ProductId").Value);

                categoriesProducts.Add(currCategoryProduct);
            });

            context.CategoryProducts.AddRange(categoriesProducts);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            //Get all products in a specified price range between 500 and 1000 (inclusive). Order them by price (from lowest to highest). Select only the product name, price and the full name of the buyer. Take top 10 records.

            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new GetProductsInRangeDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerName = $"{x.Buyer.FirstName} {x.Buyer.LastName}"
                })
                .Take(10)
                .ToArray();

            var xml = SerializeObject(products, "Products", false);
            return xml;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            //Get all users who have at least 1 sold item. Order them by last name, then by first name. Select the person's first and last name. For each of the sold products, select the product's name and price. Take top 5 records. 
            var users = context
                .Users
                .Include(x => x.ProductsSold)
                .Where(x => x.ProductsSold.Any())
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ProjectTo<GetSoldProductsDto>()
                .ToArray();

            var xml = SerializeObject(users, "Users");
            return xml;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //Get all categories. For each category select its name, the number of products, the average price of those products and the total revenue (total price sum) of those products (regardless if they have a buyer or not). Order them by the number of products (descending) then by total revenue.

            var categories = context
                .Categories
                .ProjectTo<GetCategoriesByProductsCountDto>()
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var xml = SerializeObject(categories, "Categories");
            return xml;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //Select all users who have at least 1 sold product. Order them by the number of sold products (from highest to lowest). Select only their first and last name, age, count of sold products and for each product - name and price sorted by price (descending).

            var users = context
                .Users
                .Where(x => x.ProductsSold.Any())
                .OrderByDescending(x => x.ProductsSold.Count)
                .ProjectTo<UserDto>()
                .ToArray();

            var facade = Mapper.Map<UsersAndProductsDto>(users.ToList());

            var xml = SerializeObject(facade, "Users");
            return xml;
        }

        public static string SerializeObject<T>(T values, string rootName, bool omitXmlDeclaration = false,
            bool indentXml = true)
        {
            string xml = string.Empty;

            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            var settings = new XmlWriterSettings()
            {
                Indent = indentXml,
                OmitXmlDeclaration = omitXmlDeclaration
            };

            XmlSerializerNamespaces @namespace = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, values, @namespace);
                xml = stream.ToString();
            }

            return xml;
        }
    }
}