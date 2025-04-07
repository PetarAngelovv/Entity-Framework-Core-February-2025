using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Castle.Core.Resource;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();
            // string inputXml = File.ReadAllText("../../../Datasets/sales.xml");

            Console.WriteLine(GetSalesWithAppliedDiscount(dbContext));
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;
            ImportSupplierDto[] importSupDtos = XmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

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

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;
            ImportPartDto[]? importPartDtos = XmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");
            if (importPartDtos != null)
            {
                var suppplier = context.Suppliers.Select(s => s.Id).ToArray();

                ICollection<Part> partsToAdd = new HashSet<Part>();
                foreach (var dto in importPartDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isPriceValid = decimal.TryParse(dto.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedPrice);
                    var isQuantityValid = int.TryParse(dto.Quantity, out int parsedQuantity);
                    var isSupplierIdValid = int.TryParse(dto.SupplierId, out int parsedSupplierId);

                    if (!isPriceValid || !isQuantityValid || !isSupplierIdValid)
                    {
                        continue;
                    }
                    if (!suppplier.Contains(parsedSupplierId))
                    {
                        continue;
                    }
                    Part part = new Part() {
                        Name = dto.Name,
                        Price = parsedPrice,
                        Quantity = parsedQuantity,
                        SupplierId = parsedSupplierId,
                    };
                    partsToAdd.Add(part);
                }
                context.Parts.AddRange(partsToAdd);
                context.SaveChanges();
                result = $"Successfully imported {partsToAdd.Count}";
            }

            return result;
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;
            ImportCarDto[] importCarDtos = XmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");

            if (importCarDtos != null)
            {
                ICollection<int> dbPartsId = context.Parts.Select(p => p.Id).ToArray();

                ICollection<Car> carsToAdd = new List<Car>();
                ICollection<PartCar> validPartsToAdd = new List<PartCar>();

                foreach (ImportCarDto dto in importCarDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }

                    var istraveledDistanceValid = long.TryParse(dto.TraveledDistance, out long parsedDistance);

                    if (!istraveledDistanceValid)
                    {
                        continue;
                    }

                    Car car = new Car()
                    {
                        Make = dto.Make,
                        Model = dto.Model,
                        TraveledDistance = parsedDistance,
                    };


                    if (dto.Parts != null)
                    {
                        int[] partids = dto.Parts
                                    .Where(p => IsValid(p)
                                                && int.TryParse(p.id, out int dummy))
                                    .Select(p => int.Parse(p.id))
                                    .Distinct()
                                    .ToArray();

                        foreach (var partid in partids)
                        {
                            if (!dbPartsId.Contains(partid))
                            {
                                continue;
                            }
                            PartCar partCar = new PartCar()
                            {
                                PartId = partid,
                                Car = car
                            };
                            validPartsToAdd.Add(partCar);
                        }

                    }
                    carsToAdd.Add(car);
                }
                context.Cars.AddRange(carsToAdd);
                context.PartsCars.AddRange(validPartsToAdd);
                context.SaveChanges();


                result = $"Successfully imported {carsToAdd.Count}";
            }

            return result;
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCustomerDto[] importCustDtos = XmlHelper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

            if (importCustDtos != null)
            {
                ICollection<Customer> customersToAdd = new HashSet<Customer>();
                foreach (var dto in importCustDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isbirthDateValid = DateTime.TryParse(dto.birthDate, out DateTime ParsedbirthDate);
                    var isYoungDriverValid = bool.TryParse(dto.isYoungDriver, out bool ParsedYoungDriver);

                    if (!isbirthDateValid || !isYoungDriverValid)
                    {
                        continue;
                    }

                    Customer customer = new Customer()
                    {
                        Name = dto.Name,
                        BirthDate = ParsedbirthDate,
                        IsYoungDriver = ParsedYoungDriver,
                    };
                    customersToAdd.Add(customer);
                }
                context.Customers.AddRange(customersToAdd);
                context.SaveChanges();
                result = $"Successfully imported {customersToAdd.Count}";
            }

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportSaleDto[]? importSaleDtos = XmlHelper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");

            if (importSaleDtos != null)
            {
                ICollection<int> CarIds = context.Cars.Select(c => c.Id).ToArray();

                ICollection<Sale> salesToAdd = new HashSet<Sale>();
                foreach (var dto in importSaleDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isCarIdValid = int.TryParse(dto.carId, out int parsedCarId);
                    var isCustomerIdValid = int.TryParse(dto.customerId, out int parsedCustomerId);
                    var isDiscountValid = int.TryParse(dto.discount, out int parsedDiscount);

                    if (!isCarIdValid || !isCustomerIdValid ||
                        !isDiscountValid || !CarIds.Contains(parsedCarId))
                    {
                        continue;
                    }

                    Sale sale = new Sale()
                    {
                        CarId = parsedCarId,
                        CustomerId = parsedCustomerId,
                        Discount = parsedDiscount,
                    };
                    salesToAdd.Add(sale);
                }
                context.Sales.AddRange(salesToAdd);
                context.SaveChanges();
                result = $"Successfully imported {salesToAdd.Count}";
            }
            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var xmlInput = context.Cars
                .Where(c => c.TraveledDistance > 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    traveledDistance = c.TraveledDistance

                })
                .ToArray();

            string exportCarDto = XmlHelper.Serialize(xmlInput, "cars");
            return exportCarDto;

        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var xmlInput = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new ExportCarAttributesDto()
                {
                    id = c.Id.ToString(),
                    model = c.Model,
                    traveledDistance = c.TraveledDistance
                })
                .ToArray();


            string exportCarDto = XmlHelper.Serialize(xmlInput, "cars");
            return exportCarDto;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var inputXml = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportSupplierDto()
                {
                    id = s.Id.ToString(),
                    name = s.Name,
                    partsCount = s.Parts.Count.ToString()
                })
                .ToArray();

            string xmlResult = XmlHelper.Serialize(inputXml, "suppliers");
            return xmlResult;
        }
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var inputXml = context.Cars
                .Select(c => new ExportCarAttributesDto()
                {
                    make = c.Make,
                    model = c.Model,
                    traveledDistance = c.TraveledDistance,
                    parts = c.PartsCars
                                .Select(pc => new ExportCarPartDto
                                {
                                    name = pc.Part.Name,
                                    price = pc.Part.Price
                                })
                                .OrderByDescending(p => p.price)
                                .ToArray()
                })
                .OrderByDescending(c => c.traveledDistance)
                .ThenBy(c => c.model)
                .Take(5)
                .ToArray();

            string xmlInput = XmlHelper.Serialize(inputXml, "cars");
            return xmlInput;
        }
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
       .Select(c => new
       {
           FullName = c.Name,
           BoughtCars = c.Sales.Count,
           SpentMoney = c.Sales
               .SelectMany(s => s.Car.PartsCars)
               .Sum(pc => Math.Round((double)pc.Part.Price * (c.IsYoungDriver ? 0.95 : 1), 2))
       })
       .Where(c => c.BoughtCars > 0)
       .OrderByDescending(c => c.SpentMoney)
       .AsEnumerable()
       .Select(c => new ExportCustomerDto
       {
           FullName = c.FullName,
           BoughtCars = c.BoughtCars,
           SpentMoney = Math.Round((decimal)c.SpentMoney, 2)
       })

       .ToArray();

            string XmlResult = XmlHelper.Serialize(customers, "customers");

            return XmlResult;

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
      {
            var sales = context.Sales
         .Select(s => new ExportSalesWithDiscount
         {
             Car = new CarDetails
             {
                 Make = s.Car.Make,
                 Model = s.Car.Model,
                 TraveledDistance = s.Car.TraveledDistance
             },
             Discount = (int)s.Discount,
             Customer = s.Customer.Name,
             Price = Math.Round(s.Car.PartsCars.Sum(pc => pc.Part.Price), 4),
             PriceWithDiscount = Math.Round((double)(s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100)),4)
         })
         .ToList();

            string xmlResult = XmlHelper.Serialize(sales, "sales");
            return xmlResult;
           
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
