namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
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

            //string result = GetBooksNotReleasedIn(context, input);

            // string result = GetBooksByCategory(context, input);

            //string result = GetBooksReleasedBefore(context, input);

            string input = Console.ReadLine();

            string result = GetAuthorNamesEndingIn(context, input);

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
 
            return str;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string str = String.Empty;

            string[] searchCategories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant())
                .ToArray();

            var books = context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .Where(b => b.BookCategories.Any(bc => searchCategories.Contains(bc.Category.Name.Trim().ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return str = String.Join(Environment.NewLine, books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value < DateTime.Parse(date))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new{

                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            foreach(var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price.ToString("F2")}");
            }
            Console.WriteLine(sb.ToString().Length);
            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string sb = string.Empty;

            var booksAuthors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new {
                    a.FirstName,
                    a.LastName
                   })
                .ToArray();

            sb = string.Join(Environment.NewLine, booksAuthors.Select(a => $"{a.FirstName} {a.LastName}"));

            return sb;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var BooksTitles = context.Books
                .Where(b => b.Title.Contains(input))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            foreach (var book in BooksTitles)
            {
                sb.AppendLine(book);
            }
            Console.WriteLine(sb.ToString().Length);
            return sb.ToString().TrimEnd();
        }
    }
}


