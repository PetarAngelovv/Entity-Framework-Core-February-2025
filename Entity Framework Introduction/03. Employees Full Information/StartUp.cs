using SoftUni.Data;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext DbContext = new SoftUniContext();
            DbContext.Database.EnsureCreated();

            string result = GetEmployeesFullInformation(DbContext);
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
    }
}
