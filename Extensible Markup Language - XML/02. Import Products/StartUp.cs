﻿using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext dbContext = new ProductShopContext();

            string XMLInput = File.ReadAllText("../../../Datasets/products.xml");

            Console.WriteLine(ImportProducts(dbContext, XMLInput));

        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportUserDto[] userDtos = XmlHelper.Deserialize<ImportUserDto[]>(inputXml,"Users");

            if(userDtos != null)
            {
                ICollection<User> usersToAdd = new HashSet<User>();

                foreach (var dto in userDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                   int? userAge = null;
     
                   if (dto.Age != null)
                   {
     
                       bool isAgeValid = int.TryParse(dto.Age, out int parsedAge);
                       if (!isAgeValid)
                       {
                           continue;
                       }
                       userAge = parsedAge;
                   }
                    User user = new User()
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Age = userAge
                    };
                    usersToAdd.Add(user);
                }
                context.Users.AddRange(usersToAdd);
                context.SaveChanges();
                result = $"Successfully imported {usersToAdd.Count}";
            }

            return result;
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {

            string result = string.Empty;
            ImportProductDto[]? importProductDtos = XmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

            if (importProductDtos != null)
            {
                ICollection<int> dbUsers = context.Users.Select(c => c.Id).ToArray();

                ICollection<Product> validProducts = new List<Product>();

                foreach (var dto in importProductDtos)
                {

                    if (!IsValid(dto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal.TryParse(dto.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedPrice);

                    bool IsSellerIdValid = int.TryParse(dto.SellerId, out int parsedSellerId);

                    if ((!isPriceValid) || (!IsSellerIdValid))
                    {
                        continue;
                    }

                    int? BuyerId = null;

                    if (dto.BuyerId != null)
                    {
                        bool IsBuyerIdValid = int.TryParse(dto.BuyerId, out int parsedBuyerId);
                        if (!IsBuyerIdValid)
                        {
                            continue;
                        }

                        //if (!dbUsers.Contains(parsedBuyerId))
                        //{
                        //    continue;
                        //}
                        BuyerId = parsedBuyerId;
                    }

                    //if (!dbUsers.Contains(parsedSellerId))
                    //{
                    //    continue;
                    //}
                    Product product = new Product
                    {

                        Name = dto.Name,
                        Price = parsedPrice,
                        SellerId = parsedSellerId,
                        BuyerId = BuyerId

                    };
                    validProducts.Add(product);

                }
                context.Products.AddRange(validProducts);
                context.SaveChanges();
                result = $"Successfully imported {validProducts.Count}";
            }
            return result;
        }
        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validateResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validateResults, true);

            return isValid;
        }
    }
}