using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();

            string jsonString = File.ReadAllText("../../../Datasets/parts.json");
            Console.WriteLine(ImportParts(dbContext, jsonString));
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

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportPartDto[]? importPartDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            if(importPartDtos != null)
            {

                ICollection<Part> partsToAdd = new HashSet<Part>();

                var suppliers = context.Suppliers.Select(s => s.Id);

                foreach (var dto in importPartDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }

                    bool isValidprice = decimal.TryParse(dto.Price,NumberStyles.Any, CultureInfo.InvariantCulture, out decimal ParsedPrice);
                    bool isvalidQuantity = int.TryParse(dto.Quantity, out int ParsedQuantity);
                    bool isValidSupplierId = int.TryParse(dto.SupplierId, out int ParsedId);

                    if (!isValidprice || !isvalidQuantity || !isValidSupplierId && dto.Name == null)
                    {
                        continue;
                    }

                    if (!suppliers.Contains(ParsedId))
                    {
                        continue;
                    }

                    Part part = new Part()
                    {
                        Name = dto.Name,
                        Price = ParsedPrice,
                        Quantity = ParsedQuantity,
                        SupplierId = ParsedId

                    };
                    partsToAdd.Add(part);
                }
                context.Parts.AddRange(partsToAdd);
                context.SaveChanges();
                result = $"Successfully imported {partsToAdd.Count}.";
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