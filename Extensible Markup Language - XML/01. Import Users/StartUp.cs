using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.ComponentModel.DataAnnotations;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext dbContext = new ProductShopContext();

            string xmlPath = "../../../Datasets/users.xml";

            string XMLInput = File.ReadAllText(xmlPath);

            Console.WriteLine(ImportUsers(dbContext, XMLInput));
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
                //context.SaveChanges();
                result = $"Successfully imported {usersToAdd.Count}";
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