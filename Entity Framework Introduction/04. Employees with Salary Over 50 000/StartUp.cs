﻿using SoftUni.Data;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
        // SoftUniContext DbContext = new SoftUniContext();
        // DbContext.Database.EnsureCreated();
        //
        // string result = GetEmployeesFullInformation(DbContext);
        // Console.WriteLine(result);

            SoftUniContext DbContext = new SoftUniContext();
            DbContext.Database.EnsureCreated();

            string result = GetEmployeesWithSalaryOver50000(DbContext);
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
    }
}
