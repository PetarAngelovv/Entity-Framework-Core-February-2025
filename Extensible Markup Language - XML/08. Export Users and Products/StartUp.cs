using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext dbContext = new ProductShopContext();

            //  string XMLInput = File.ReadAllText("../../../Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(dbContext));

        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportUserDto[] userDtos = XmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

            if (userDtos != null)
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

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;
            ImportCategoryDto[] importCategoryDtos = XmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            if (importCategoryDtos != null)
            {
                ICollection<Category> categoriesToAdd = new HashSet<Category>();
                foreach (var dto in importCategoryDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    if (dto.Name == null)
                    {
                        continue;
                    }

                    Category category = new Category() { Name = dto.Name };
                    categoriesToAdd.Add(category);
                }
                context.Categories.AddRange(categoriesToAdd);
                context.SaveChanges();
                result = $"Successfully imported {categoriesToAdd.Count}";
            }
            return result;
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;
            ImportCategoryProductDto[]? importCatProdDtos = XmlHelper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");
            if (importCatProdDtos != null)
            {
                ICollection<CategoryProduct> categoryProductsToAdd = new HashSet<CategoryProduct>();

                ICollection<int> dbProducts = context
                           .Products
                           .Select(p => p.Id).ToArray();

                ICollection<int> dbCategories = context
                         .Categories
                         .Select(c => c.Id)
                         .ToArray();

                foreach (var dto in importCatProdDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var IsCategoryIdValid = int.TryParse(dto.CategoryId, out int parsedCategoryId);
                    var IsProductIdValid = int.TryParse(dto.ProductId, out int parsedProductId);

                    if (!IsCategoryIdValid || !IsProductIdValid)
                    {
                        continue;
                    }
                    CategoryProduct product = new CategoryProduct()
                    {
                        CategoryId = parsedCategoryId,
                        ProductId = parsedProductId

                    };
                    categoryProductsToAdd.Add(product);
                }
                context.CategoryProducts.AddRange(categoryProductsToAdd);
                context.SaveChanges();
                result = $"Successfully imported {categoryProductsToAdd.Count}";
            }
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var product = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductDto()
                {

                    Name = p.Name,
                    Price = p.Price,
                    Buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"

                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            string XMLresult = XmlHelper.Serialize<ExportProductDto[]>(product, "Products");
            return XMLresult;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
                    var users = context.Users
               .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
               .OrderBy(u => u.LastName)
               .ThenBy(u => u.FirstName)
               .Select(u => new ExportUserDto
               {
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   SoldProducts = new ExportSoldProductsDto
                   {
                       Count = u.ProductsSold.Count(ps => ps.BuyerId != null),
                       Products = u.ProductsSold
                           .Where(ps => ps.BuyerId != null)
                           .Select(ps => new ExportProductDto
                           {
                               Name = ps.Name,
                               Price = ps.Price
                           })
                           .ToArray()
                   }
               })
               .Take(5)
               .ToArray();

                    return XmlHelper.Serialize(users, "Users").Trim();

        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var rawData = context.Categories
             .Select(c => new
             {
                 Name = c.Name,
                 Count = c.CategoryProducts.Count,
                 AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                 TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
             })
             .ToArray()
             .OrderByDescending(c => c.Count)
             .ThenBy(c => c.TotalRevenue)
             .Select(c => new ExportCategoryDto
             {
                 Name = c.Name,
                 Count = c.Count,
                 AveragePrice = c.AveragePrice,
                 TotalRevenue = c.TotalRevenue
             })
            
             .ToArray();

            return XmlHelper.Serialize(rawData, "Categories");


        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var filteredUsers = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null));

            var users = filteredUsers
                .OrderByDescending(u => u.ProductsSold.Count(ps => ps.BuyerId != null))
                .Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsDto
                    {
                        Count = u.ProductsSold.Count(ps => ps.BuyerId != null),
                        Products = u.ProductsSold
                                    .Where(ps => ps.BuyerId != null)
                                    .Select(p => new ExportProductDto
                                    {
                                        Name = p.Name,
                                        Price = p.Price
                                    })
                                    .OrderByDescending(p => p.Price)
                                    .ToArray()
                    }
                })
                .Take(10)
                .ToArray();


            var resultDto = new ExportUserWithProductDto
            {
                Count = filteredUsers.Count(),
                Users = users
            };

            string xmlResult = XmlHelper.Serialize(resultDto, "Users");
            return xmlResult.Trim();
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