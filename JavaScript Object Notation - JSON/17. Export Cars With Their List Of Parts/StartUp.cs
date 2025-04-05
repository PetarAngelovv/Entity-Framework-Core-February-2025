using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

            string jsonString = File.ReadAllText("../../../Datasets/cars.json");

            Console.WriteLine(GetCarsWithTheirListOfParts(dbContext));
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

        //   public static string ImportCars(CarDealerContext context, string inputJson)
        //{
        //    string result = string.Empty;

        //    ImportCarDto[]? importCarDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

        //    if (importCarDtos != null)
        //    {
        //        var parts = context.Parts.Select(p => p.Id).ToList(); 
        //        ICollection<Car> carsToAdd = new List<Car>();

        //        foreach (var dto in importCarDtos)
        //        {
        //            if (!IsValid(dto))
        //            {
        //                continue;
        //            }

        //            var isValidTraveledDistance = int.TryParse(dto.TraveledDistance, out int parsedDistance);

        //            if (!isValidTraveledDistance)
        //            {
        //                continue; 
        //            }


        //            List<int> parsedPartIds = dto.PartsId
        //                .Select(id => int.TryParse(id, out int parsed) ? parsed : (int?)null)
        //                .Where(id => id.HasValue && parts.Contains(id.Value)) 
        //                .Select(id => id.Value)
        //                .Distinct() 
        //                .ToList();


        //            if (parsedPartIds.Count != dto.PartsId.Length)
        //            {
        //                continue;
        //            }

        //            Car car = new Car()
        //            {
        //                Make = dto.Make,
        //                Model = dto.Model,
        //                TraveledDistance = parsedDistance,
        //                PartsCars = parsedPartIds
        //                    .Select(partId => new PartCar { PartId = partId })
        //                    .ToList()
        //            };

        //            carsToAdd.Add(car); 
        //        }


        //        context.Cars.AddRange(carsToAdd);
        //        context.SaveChanges();

        //        result = $"Successfully imported {carsToAdd.Count}.";
        //    }

        //    return result;
        //}

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            var carDTOs = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);
            var cars = mapper.Map<Car[]>(carDTOs);

            // Вземи всички съществуващи PartId от таблицата Parts
            var validPartIds = context.Parts.Select(p => p.Id).ToHashSet();

            int partsCarsCount = 0; // броене на добавените записи

            foreach (var carDTO in carDTOs)
            {
                var car = mapper.Map<Car>(carDTO);

                foreach (var partId in carDTO.PartsId.Distinct())
                {
                    // Проверка дали PartId съществува в таблицата Parts
                    if (!validPartIds.Contains(partId))
                    {
                        continue; // Пропускай запис с невалиден PartId
                    }

                    context.PartsCars.Add(new PartCar
                    {
                        PartId = partId,
                        Car = car
                    });

                    partsCarsCount++;  // Увеличаваме броя за всеки добавен запис
                }
            }

            context.Cars.AddRange(cars);
          //context.SaveChanges();

            return $"Successfully imported {cars.Length}.";

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

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;
            ImportSaleDto[]? importSaleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);
            if (importSaleDtos != null)
            {
                ICollection<Sale> salesToAdd = new List<Sale>();

                foreach(var dto in importSaleDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isValidCarId = int.TryParse(dto.carId, out int ParsedCarId);

                    var isValidCustomerId = int.TryParse(dto.customerId, out int ParsedCustomerId);

                    var isValidDiscount = int.TryParse(dto.Discount, out int ParsedDiscount);

                    if(!isValidCarId || !isValidCustomerId || !isValidDiscount)
                    {
                        continue;
                    }

                    Sale sale = new Sale()
                    {
                        CarId = ParsedCarId,
                        CustomerId = ParsedCustomerId,
                        Discount = ParsedDiscount,

                    };
                    salesToAdd.Add(sale);
                }
                context.Sales.AddRange(salesToAdd);
                context.SaveChanges();
                result = $"Successfully imported {salesToAdd.Count}.";
            } 
            return result;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                 .OrderBy(c => c.BirthDate) // First, order by birth date
                 .ThenBy(c => c.IsYoungDriver)
                .Select(c => new { 
                c.Name,
                BirthDate =  c.BirthDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture),
                c.IsYoungDriver
                })
                
                .ToArray();

            string jsonResult = JsonConvert.SerializeObject(customers, Formatting.Indented);


            return jsonResult;
          
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance

                })
                .OrderBy(c =>c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            string jsonResult = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return jsonResult;
               

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            string jsonResult = JsonConvert.SerializeObject (suppliers, Formatting.Indented);
            return jsonResult;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
         .Select(c => new
         {
             car = new
             {
                 c.Make,
                 c.Model,
                 c.TraveledDistance
             },
             parts = context.PartsCars
                 .Where(pc => pc.CarId == c.Id)
                 .Select(pc => new
                 {
                     pc.Part.Name,
                     Price = pc.Part.Price.ToString("F2")  // Форматираме цената до 2 знака след десетичната запетая
                 }).ToArray()
         })
         .ToArray();

            string jsonResult = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);
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