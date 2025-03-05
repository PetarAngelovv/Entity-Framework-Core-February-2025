using Microsoft.EntityFrameworkCore;

namespace MusicHub
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Data;
    using MusicHub.Data.Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
             new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            string result = ExportAlbumsInfo(context, 9);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
        .Where(a => a.ProducerId == producerId)
        .ToList() // Принудително зареждане в паметта, за да може да се изчисли Price
        .OrderByDescending(a => a.Price) // Сега можем да сортираме по Price
        .Select(a => new
        {
            a.Name,
            ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
            ProducerName = a.Producer.Name,
            Songs = a.Songs
                .OrderByDescending(s => s.Name)
                .ThenBy(s => s.Writer.Name)
                .Select((s, index) => new
                {
                    Index = index + 1,
                    s.Name,
                    Price = s.Price.ToString("F2"),
                    WriterName = s.Writer.Name
                })
                .ToList(),
            TotalPrice = a.Price.ToString("F2") 
        })
        .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var a in albums)
            {
                sb.AppendLine($"-AlbumName: {a.Name}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");

                foreach (var s in a.Songs)
                {
                    sb.AppendLine($"---#{s.Index}");
                    sb.AppendLine($"---SongName: {s.Name}");
                    sb.AppendLine($"---Price: {s.Price}");
                    sb.AppendLine($"---Writer: {s.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {a.TotalPrice}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
