using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //var inputJson = File.ReadAllText(@"D:\GitHub\Softuni-CSharp-DB-Fundamentals\Databases Advanced - Entity Framework\Exercise JSON Processing\ProductShop\ProductShop\Datasets\categories-products.json");
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);
            var validUsers = new List<User>();

            foreach (var user in users)
            {
                if (user.LastName.Length < 3)
                {
                    continue;
                }

                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson);
            var validCategories = new List<Category>();

            foreach (var category in categories)
            {
                if (category.Name == null ||
                    category.Name.Length < 3 ||
                    category.Name.Length > 15)
                {
                    continue;
                }

                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);
            var validEntities = new List<CategoryProduct>(categoriesProducts);

            context.CategoryProducts.AddRange(validEntities);
            context.SaveChanges();

            return $"Successfully imported {validEntities.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    Name = x.Name,
                    Price = x.Price,
                    Seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToList();
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(products,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver
                });

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var result = context
                .Users
                .Where(x => x.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                        .Where(y => y.Buyer != null)
                        .Select(y => new
                        {
                            Name = y.Name,
                            Price = y.Price,
                            BuyerFirstName = y.Buyer.FirstName,
                            BuyerLastName = y.Buyer.LastName
                        }).ToList()
                }).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(result,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver
                });

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context
                .Categories
                .OrderByDescending(x => x.CategoryProducts.Count)
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Select(y => y.Product.Price).Average():f2}",
                    TotalRevenue = $"{x.CategoryProducts.Select(y => y.Product.Price).Sum():f2}"
                }).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(result,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver
                });

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //var result = context
            //    .Users
            //    .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
            //    .OrderByDescending(x => x.ProductsSold
            //                             .Where(y => y.Buyer != null)
            //                             .Count())
            //    .Select(x => new
            //    {
            //        UsersCount = context.Users
            //                       .Where(y => y.ProductsSold.Any(ps => ps.Buyer != null))
            //                       .Count(),
            //        Users = context.Users
            //                       .Where(y => y.ProductsSold.Any(ps => ps.Buyer != null))
            //                       .Select(y => new
            //                       {
            //                           y.LastName,
            //                           y.Age,
            //                           SoldProducts = y.ProductsSold.Select(ps => new
            //                           {
            //                               Count = y.ProductsSold.Count,
            //                               Products = y.ProductsSold
            //                               //.Where(u => u.Name.Length >= 3)
            //                               .Select(u => new { u.Name, u.Price })
            //                               //Products = new
            //                               //{
            //                               //    ps.Name,
            //                               //    ps.Price
            //                               //}
            //                           }).Take(1)
            //                       })
            //        //Users = context.Users
            //        //               .Where(u => u.LastName != null &&
            //        //                           u.Age != null &&
            //        //                           u.ProductsSold != null)
            //        //               .Select(u => new
            //        //               {
            //        //                   LastName = u.LastName,
            //        //                   Age = u.Age,
            //        //                   SoldProducts = u.ProductsSold
            //        //                   .Select(ps => new
            //        //                   {
            //        //                       Count = u.ProductsSold.Count
            //        //                       //TODO
            //        //                   })
            //        //               })
            //    }).Take(1).Distinct().ToList().Distinct();

            var users = context
                .Users
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .OrderByDescending(x => x.ProductsSold
                                         .Count(y => y.Buyer != null))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new
                    {
                        Count = x.ProductsSold
                            .Count(ps => ps.Buyer != null),
                        Products = x.ProductsSold
                            .Where(ps => ps.Buyer != null)
                            .Select(ps => new
                            {
                                Name = ps.Name,
                                Price = ps.Price
                            })
                            .ToArray()
                    }
                }).ToArray();

            var result = new
            {
                UsersCount = users.Length,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(result,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver,
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    }
}