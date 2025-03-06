namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        { 
            //  DbInitializer.ResetDatabase(db);
            using var context = new BookShopContext();
            //string input = Console.ReadLine();
            //string result = GetBooksByAgeRestriction(context, input);

            string result = GetGoldenBooks(context);
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
    }
}


