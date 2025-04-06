using Newtonsoft.Json;
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

            string XMLInput = File.ReadAllText("../../../Datasets/categories-products.xml");

            Console.WriteLine(ImportCategoryProducts(dbContext, XMLInput));

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

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;
            ImportCategoryDto[] importCategoryDtos = XmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            if(importCategoryDtos != null)
            {
                    ICollection<Category> categoriesToAdd = new HashSet<Category>();
                foreach (var dto in importCategoryDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    if(dto.Name == null)
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
            if(importCatProdDtos != null)
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
                    var IsCategoryIdValid = int.TryParse(dto.CategoryId,out int parsedCategoryId);
                    var IsProductIdValid = int.TryParse(dto.ProductId, out int parsedProductId);

                    if(!IsCategoryIdValid || !IsProductIdValid)
                    {
                        continue ;
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
        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validateResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validateResults, true);

            return isValid;
        }
    }
}