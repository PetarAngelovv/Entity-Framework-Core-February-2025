using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace Exercises_Entity_Relations
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<StudentSystemContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=Student_System;Trusted_Connection=True;");

            using (var context = new StudentSystemContext(optionsBuilder.Options))
            {

                context.Database.Migrate(); // Това ще създаде базата данни и таблиците, ако още не съществуват
            }

            Console.WriteLine("Db is created!");
        }
    }
}
