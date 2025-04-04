using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();

            string jsonString = File.ReadAllText("../../../Datasets/suppliers.json");
            Console.WriteLine(ImportSuppliers(dbContext, jsonString));
        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportSupplierDto[]? supplierDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

            if (supplierDtos != null)
            {
                ICollection<Supplier> suppliersToAdd = new HashSet<Supplier>();
                foreach(var dto in supplierDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    if (dto.Name == null)
                    {
                        continue;
                    }
                    bool isValidImporter = bool.TryParse(dto.isImporter, out bool ParsedImporter);

                    if (!isValidImporter)
                    {
                        continue;
                    }

                    Supplier supplier = new Supplier() {
                        
                        Name = dto.Name,
                        IsImporter = ParsedImporter
                    
                    };
                    suppliersToAdd.Add(supplier);
                }
                context.Suppliers.AddRange(suppliersToAdd);
                context.SaveChanges();
                result = $"Successfully imported {suppliersToAdd.Count}.";
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