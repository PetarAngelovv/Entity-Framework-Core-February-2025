using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext dbContext = new ProductShopContext();

            Console.WriteLine(GetProductsInRange(dbContext));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;
            ImportUserDto[]? UserDTOs = JsonConvert.
                DeserializeObject<ImportUserDto[]>(inputJson);

            if (UserDTOs != null)
            {
                ICollection<User> usersToAdd = new List<User>();
                foreach (ImportUserDto dto in UserDTOs)
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

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {

            string result = string.Empty;

            ImportProductDto[]? importProductsDTO = JsonConvert.
                DeserializeObject<ImportProductDto[]>(inputJson);

            if (importProductsDTO != null)
            {
                ICollection<int> dbUsers = context.Users.Select(c => c.Id).ToArray();

                ICollection<Product> validProducts = new List<Product>();

                foreach (var dto in importProductsDTO)
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

                           if (!dbUsers.Contains(parsedBuyerId))
                           {
                               continue;
                           }
                        BuyerId = parsedBuyerId;
                    }

                          if (!dbUsers.Contains(parsedSellerId))
                          {
                              continue;
                          }
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

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;
            ImportCategoryDto[]? importCategoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);
            if(importCategoryDtos != null)
            {
                ICollection<Category> categoriesToAdd = new List<Category>();

                foreach (var dto in importCategoryDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                  
                    Category category = new Category
                    {
                        Name = dto.Name!
                    };
                    categoriesToAdd.Add(category);
  
                }

                context.Categories.AddRange(categoriesToAdd);
                context.SaveChanges();
                result = $"Successfully imported {categoriesToAdd.Count}";
            }
            
            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCategoryProductDto[]? catProdDtos = JsonConvert
                                                .DeserializeObject<ImportCategoryProductDto[]>(inputJson); 

            if(catProdDtos != null)
            {
                ICollection<CategoryProduct> validCatProd = new List<CategoryProduct>();

                ICollection<int> dbProducts = context
                    .Products
                    .Select(p => p.Id).ToArray();

                ICollection<int> dbCategories = context
                    .Categories
                    .Select(c => c.Id)
                    .ToArray();

                foreach (var catProdDto in catProdDtos)
                {
                    if (!IsValid(catProdDto))
                    {
                        continue;
                    }

                    bool isProductIdValid = int.TryParse(catProdDto.ProductId, out int productId);

                    bool isCategoryIdValid = int.TryParse(catProdDto.CategoryId, out int categoryId);

                    if (!isProductIdValid || !isCategoryIdValid )
                    {
                        continue;
                    }

                    CategoryProduct catProd = new CategoryProduct()
                    {
                        ProductId = productId,
                        CategoryId = categoryId

                    };
                    validCatProd.Add(catProd);
                }
                context.CategoriesProducts.AddRange(validCatProd);
                context.SaveChanges();
                result = $"Successfully imported {validCatProd.Count}";
            }
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
            .Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .Select(p => new {
                  p.Name,
                  p.Price,
                Seller = $"{p.Seller.FirstName} {p.Seller.LastName}",
            })
            .OrderBy(p => p.Price)
            .ToArray();

            DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert.SerializeObject(products, Formatting.Indented, new JsonSerializerSettings 
            {  
            ContractResolver = camelCaseResolver
            });


            return jsonResult;
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







