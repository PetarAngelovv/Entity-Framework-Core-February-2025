namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        { 
            //  DbInitializer.ResetDatabase(db);
            using var context = new BookShopContext();

            //string result = GetBooksByAgeRestriction(context, input);

            //string result = GetGoldenBooks(context);

            //string result = GetBooksByPrice(context);
            int input = int.Parse(Console.ReadLine());
            string result = GetBooksNotReleasedIn(context, input);
        
            Console.WriteLine(result);
          
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            string str = string.Empty;
            bool isValid = Enum.TryParse(command, true, out AgeRestriction ageRestriction);

            if (!isValid)
            {
                return str;
            }

            string[] booksTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            str = String.Join(Environment.NewLine, booksTitles);

            return str;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            string str = string.Empty;

            bool isGold = Enum.TryParse("gold",true, out EditionType editionType);
            var BookTitles = context.Books
                .Where(b => b.EditionType == editionType &&
                  b.Copies < 5000)
                 .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            str = String.Join(Environment.NewLine,BookTitles);

            return str;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books
                .Where(b=> b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new {
                    b.Title,
                    b.Price
                })
                .ToArray();

            foreach(var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price.ToString("F2")}");
            }
            
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string str = string.Empty;

            var booksTitles = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            str = String.Join(Environment.NewLine,booksTitles);
            Console.WriteLine(str.Length);

            return str;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string str = String.Empty;


            return str;
        }

    }
}


