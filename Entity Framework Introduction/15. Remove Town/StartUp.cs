﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SoftUni.Data;
using SoftUni.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

            // string result = GetAddressesByTown(DbContext);

            // string result = GetEmployee147(DbContext);

            // string result = GetDepartmentsWithMoreThan5Employees(DbContext);

            // string result = GetLatestProjects(DbContext);

            // string result = IncreaseSalaries(DbContext);

            // string result = GetEmployeesByFirstNameStartingWithSa(DbContext);
            
            // string result = DeleteProjectById(DbContext);

            string result = RemoveTown(DbContext);
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
                .Take(10)
                .ToArray();
            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employee = context
         .Employees
         .Where(e => e.EmployeeId == 147)
         .Select(e => new
         {
             e.FirstName,
             e.LastName,
             e.JobTitle,
             EmployeesProjects = e.EmployeesProjects
                 .Select(ep => ep.Project.Name)
                 .OrderBy(projectName => projectName)
                 .ToArray()
        })
         .FirstOrDefault();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var projectName in employee.EmployeesProjects)
            {
                sb.AppendLine(projectName);
            }

            return sb.ToString().TrimEnd();

               

    }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employee = d.Employees
                        .Select(e => new {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle
                        })
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .ToArray()

                });

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");
                foreach(var e in d.Employee)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .ToArray();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }
            return sb.ToString().TrimEnd();

        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        Salary = e.Salary + (e.Salary * Convert.ToDecimal(0.12)),
                        DepartmentName = e.Department.Name
                    })
                    .Where(e => e.DepartmentName == "Engineering" ||
                     e.DepartmentName == "Tool Design" ||
                     e.DepartmentName == "Marketing" ||
                     e.DepartmentName == "Information Services")
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray();

                     foreach (var e in employees)
                     {
                             sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary.ToString("F2")})");
                     }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary.ToString("F2")})");
            }
            return sb.ToString().TrimEnd();

        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            int idForDelete = 2;
            var projectToDelete = context.Projects.Find(idForDelete);
            if (projectToDelete != null)
            {
                var relationRecords = context.EmployeesProjects.Where(ep => ep.ProjectId == idForDelete);
                context.EmployeesProjects.RemoveRange(relationRecords);

                context.Projects.Remove(projectToDelete);
                context.SaveChanges();
            }
           
         
            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToArray();

            foreach(var p in projects)
            {
                sb.AppendLine(p);

            }
            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            string townName = "Seattle";

            // Find the town object with the given name
            var town = context.Towns.FirstOrDefault(t => t.Name == townName);
            if (town == null)
            {
                return "Town not found.";
            }

            // Get all addresses associated with the town
            var addressesToDelete = context.Addresses
                                           .Where(a => a.TownId == town.TownId)
                                           .ToArray();
            int deletedAddressesCount = addressesToDelete.Length;

            // For each address, update any employee living there by setting AddressId to null
            foreach (var address in addressesToDelete)
            {
                var employeesAtAddress = context.Employees
                                                .Where(e => e.AddressId == address.AddressId)
                                                .ToList();
                foreach (var employee in employeesAtAddress)
                {
                    employee.AddressId = null;
                }
            }

            // Remove the addresses
            context.Addresses.RemoveRange(addressesToDelete);

            // Remove the town
            context.Towns.Remove(town);

            // Persist all changes to the database in one go (or in multiple SaveChanges calls if needed)
            context.SaveChanges();

            // Build and return the result message
            sb.Append($"{deletedAddressesCount} addresses in Seattle were deleted");
            return sb.ToString().TrimEnd();
        }


    }

}
