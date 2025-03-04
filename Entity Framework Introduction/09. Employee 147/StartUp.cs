using SoftUni.Data;
using SoftUni.Models;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
           SoftUniContext DbContext = new SoftUniContext();
           DbContext.Database.EnsureCreated();
        
        // string result = GetEmployeesFullInformation(DbContext);
        
        // string result = GetEmployeesWithSalaryOver50000(DbContext);
        
        // string result = GetEmployeesFromResearchAndDevelopment(DbContext);
        
        // string result = AddNewAddressToEmployee(DbContext);

        // string result = GetEmployeesInPeriod(DbContext);
           
           string result = GetAddressesByTown(DbContext);
           Console.WriteLine(result);
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Select(e => new { 
                 e.FirstName,
                 e.LastName,
                 e.MiddleName,
                 e.JobTitle,
                e.Salary
                })
                .ToArray();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary.ToString("F2")}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context) 
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50_000)
                .OrderBy(e => e.FirstName)
                .ToArray();
            foreach(var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary.ToString("F2")}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new{
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }
                return sb.ToString().TrimEnd();
        }

        //should see AGAIN
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            const string newAdressText = "Vitoshka 15";
            const int newTownId = 4;

            Address newAdress = new Address()
            {
                AddressText = newAdressText,
                TownId = newTownId
            };

            var Nakovemployees = context.Employees
                            .First(e => e.LastName.Equals("Nakov"));

            Nakovemployees.Address = newAdress;

            context.SaveChanges();



            string?[] Addresses = context
                .Employees
                .Where(e=> e.AddressId.HasValue)
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address!.AddressText)
                .Take(10)
                .ToArray();
            
            return String.Join(Environment.NewLine, Addresses);

        }
        //should see AGAIN
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                    .Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        ManagerFirstName = e.Manager == null ? null : e.Manager.FirstName,
                        ManagerLastName = e.Manager == null ? null : e.Manager.LastName,
                        Projects = e.EmployeesProjects
                        .Select(ep => ep.Project)
                        .Where(p => p.StartDate.Year >= 2001 &&
                                    p.StartDate.Year <= 2003)
                        .Select(p => new
                        {
                            ProjectName = p.Name,
                            p.StartDate,
                            p.EndDate
                        })
                        .ToArray()
                    })
                    .Take(10)
                    .ToArray();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects) 
                {
                    string startDateFormat = p.StartDate.ToString("M/d/yyyy h:mm:ss tt");
                    string EndDateFormat = p.EndDate.HasValue ? p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";
                    sb.AppendLine($"--{p.ProjectName} - {startDateFormat} - {EndDateFormat}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var addresses = context
                .Addresses
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeeCount = a.Employees.Count()


                })
                .OrderByDescending(a => a.EmployeeCount)
                .ThenBy(e => e.TownName)
                .Take(10);
            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
            }
            return sb.ToString().TrimEnd();
        }
    }


}
