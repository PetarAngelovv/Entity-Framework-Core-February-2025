namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        { 
            //  DbInitializer.ResetDatabase(db);
            using var context = new BookShopContext();

            // string result = GetBooksByAgeRestriction(context, input);

            // string result = GetGoldenBooks(context);

            // string result = GetBooksByPrice(context);

            // string result = GetBooksNotReleasedIn(context, input);

            // string result = GetBooksByCategory(context, input);

            // string result = GetBooksReleasedBefore(context, input);

            // string result = GetBookTitlesContaining(context, input);

            // string result = GetBooksByAuthor(context, input);

            // int result = CountBooks(context, input);

            // int input = int.Parse(Console.ReadLine());

            // string result = CountCopiesByAuthor(context);

            // string result = GetTotalProfitByCategory(context);

            // string result = GetMostRecentBooks(context);
             IncreasePrices(context);

           // int result = RemoveBooks(context);
           // Console.WriteLine(result);

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

            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower())) 
                .OrderBy(b => b.Title)  
                .Select(b => b.Title)  
                .ToArray();  

            foreach (var title in books)
            {
                sb.AppendLine(title);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Include(b =>b.Author)
                .Where(b => b.Author.LastName.StartsWith(input))
            .OrderBy(b =>b.BookId)
            .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
            .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }
            return sb.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int result = 0;
            var titles = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => b.Title)
                .ToArray();

            result = titles.Length;
            return result;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var authors = context.Authors
                .Include(a => a.Books)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    BooksCopies = a.Books
                    .Select(b => b.Copies)
                    .ToArray(),
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalCopies)
                .ToArray();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.TotalCopies}");
            }
                return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                              .Sum(cb => cb.Book.Price * cb.Book.Copies)

                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c =>c.Name)
                .ToArray();

            foreach (var c in categories)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:F2}");
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)

                .Select(c => new
                {
                    c.Name,
                    TopBooks = c.CategoryBooks
                                .Select(cb => new {
                                    cb.Book.ReleaseDate,
                                    cb.Book.Title
                                })
                                .OrderByDescending(b =>b.ReleaseDate)
                                .Take(3)
                                .ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();
            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.Name}");
            
                foreach(var b in c.TopBooks)
                {
                    sb.AppendLine($"{b.Title} ({b.ReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var booksToModify = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in booksToModify)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {

            return 0;
        }
    }
}


