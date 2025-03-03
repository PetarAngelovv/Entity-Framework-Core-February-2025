using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data;

namespace P02_FootballBetting
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FootballBettingContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=FootBallBetting;Trusted_Connection=True;");

            using (var context = new FootballBettingContext(optionsBuilder.Options))
            {

                context.Database.Migrate();
            }

            Console.WriteLine("Db is created!");
        }
    }
}
