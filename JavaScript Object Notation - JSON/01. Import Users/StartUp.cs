using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {

            using ProductShopContext dbContext = new ProductShopContext();

            string jsonString = File.ReadAllText("../../../Datasets/users.json");

            ImportUsers(dbContext, jsonString);

        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;
            ImportUserDTO[]? UserDTOs = JsonConvert.
                DeserializeObject<ImportUserDTO[]>(inputJson);

            if(UserDTOs != null)
            {
                ICollection<User> usersToAdd = new List<User>();
                foreach (ImportUserDTO dto in UserDTOs)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }

                    int? userAge = null;

                    if (dto.Age != null)
                    {

                        bool isAgeValid = int.TryParse(dto.Age, out int parsedAge);
                        if(!isAgeValid)
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

        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validateResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto,validateContext,validateResults,true);
            
            return isValid;
        }

    }
}