using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();
            string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");

            Console.WriteLine(ImportSuppliers(dbContext, inputXml));
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;
            ImportSupplierDto[] importSupDtos = XmlHelper.Deserialize<ImportSupplierDto[]>(inputXml,"Suppliers");

            if (importSupDtos != null)
            {
                ICollection<Supplier> suppliersToAdd = new HashSet<Supplier>();

                foreach (var dto in importSupDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    Supplier supplier = new Supplier()
                    {
                        Name = dto.Name,
                        IsImporter = dto.isImporter/*ParsedImporter*/
                    };

                    suppliersToAdd.Add(supplier);
                }
                context.AddRange(suppliersToAdd);
                context.SaveChanges();
                result = $"Successfully imported {suppliersToAdd.Count}";
            }


           
            return result;
        }

        //public static string ImportParts(CarDealerContext context, string inputXml)
        //{

        //}
        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validateResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validateResults, true);

            return isValid;
        }
    }
}