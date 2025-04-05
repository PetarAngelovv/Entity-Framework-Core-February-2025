using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();

            string jsonString = File.ReadAllText("../../../Datasets/customers.json");
            Console.WriteLine(ImportCustomers(dbContext, jsonString));
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

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCarDto[]? importCarDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            if (importCarDtos != null)
            {
                var parts = context.Parts.Select(p => p.Id).ToList();
                ICollection<Car> carsToAdd = new List<Car>();

                foreach (var dto in importCarDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isValidTraveledDistance = int.TryParse(dto.TraveledDistance, out int ParsedDistance);

                    if (!isValidTraveledDistance)
                    {
                        continue;
                    }

                    List<int> parsedPartIds = dto.PartsId
                         .Select(id => int.TryParse(id, out int parsed) ? parsed : (int?)null)
                         .Where(id => id.HasValue && parts.Contains(id.Value))
                         .Select(id => id.Value)
                         .ToList();


                    bool isValidPartsIds = parsedPartIds.All(id => parts.Contains(id));

                    if (parsedPartIds.Count != dto.PartsId.Count)
                    {
                        continue;
                    }

                    Car car = new Car()
                    {
                        Make = dto.Make,
                        Model = dto.Model,
                        TraveledDistance = ParsedDistance,
                        PartsCars = parsedPartIds
                                         .Distinct()
                                         .Select(partId => new PartCar { PartId = partId })
                                         .ToList()
                    };
                    carsToAdd.Add(car);

                }
                context.Cars.AddRange(carsToAdd);
                context.SaveChanges();
                result = $"Successfully imported {carsToAdd.Count}.";
            }
            return result;
        }

         public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;
            ImportCustomerDto[]? importCustomerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            if (importCustomerDtos != null) {

                ICollection<Customer> customersToAdd = new List<Customer>();

                foreach (var dto in importCustomerDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }

                    var isValidBirthDate = DateTime.TryParse(dto.birthDate, out DateTime birthDate);

                    var IsValidYoungDriver = bool.TryParse(dto.isYoungDriver, out bool isYoungDriver);

                    if (!isValidBirthDate || !IsValidYoungDriver)
                    {
                        continue;
                    }

                    Customer customer = new Customer()
                    { 
                        Name = dto.Name,
                        BirthDate = birthDate,
                        IsYoungDriver = isYoungDriver,
                    };
                    customersToAdd.Add(customer);
                }

                context.Customers.AddRange(customersToAdd);
                context.SaveChanges();
                result = $"Successfully imported {customersToAdd.Count}.";
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