namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var context = new BookShopContext();
            string input = Console.ReadLine();
            string result = GetBooksByAgeRestriction(context, input);
            Console.WriteLine(result);
            //  DbInitializer.ResetDatabase(db);
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
    }
}


