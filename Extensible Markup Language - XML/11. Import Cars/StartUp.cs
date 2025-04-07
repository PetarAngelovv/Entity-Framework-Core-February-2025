using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
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
            string inputXml = File.ReadAllText("../../../Datasets/cars.xml");

            Console.WriteLine(ImportCars(dbContext, inputXml));
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

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;
            ImportPartDto[]? importPartDtos = XmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");
            if (importPartDtos != null)
            {
                var suppplier = context.Suppliers.Select(s => s.Id).ToArray();

                ICollection<Part> partsToAdd = new HashSet<Part>();
                foreach(var dto in importPartDtos)
                {
                    if (!IsValid(dto))
                    {
                        continue;
                    }
                    var isPriceValid = decimal.TryParse(dto.Price,NumberStyles.Number,CultureInfo.InvariantCulture, out decimal parsedPrice);
                    var isQuantityValid = int.TryParse(dto.Quantity, out int parsedQuantity);
                    var isSupplierIdValid = int.TryParse(dto.SupplierId, out int parsedSupplierId);

                    if(!isPriceValid || !isQuantityValid || !isSupplierIdValid)
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

            if(importCarDtos != null)
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
                            if(!dbPartsId.Contains(partid))
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
               // context.Cars.AddRange(carsToAdd);
                context.PartsCars.AddRange(validPartsToAdd);
                context.SaveChanges();


                result = $"Successfully imported {carsToAdd.Count}";
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