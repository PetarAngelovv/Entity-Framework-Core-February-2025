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

            //string result = ExportAlbumsInfo(context, 9);
            string result = ExportSongsAboveDuration(context, 4);

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
            //MY CODE 

            //StringBuilder sb = new StringBuilder();
            //var producer = context.Producers
            //     .Where(p => p.Id == producerId);



            //foreach (var p in producer)
            //{
            //    foreach (var a in p.Albums.OrderByDescending(a => a.Price))
            //    {
            //        int index = 1;
            //        sb.AppendLine($"-AlbumName: {a.Name}");                                 //   -AlbumName: Devil's advocate
            //        sb.AppendLine($"-ReleaseDate: {a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}"); //   - ReleaseDate: 07 / 21 / 2018
            //        sb.AppendLine($"-ProducerName: {a.Producer.Name}");                     //   - ProducerName: Evgeni Dimitrov
            //        sb.AppendLine($"-Songs:");                                              //   -Songs:


            //        foreach (var s in a.Songs
            //                    .OrderByDescending(s => s.Name)
            //                    .ThenBy(s => s.Writer.Name))
            //        {
            //            sb.AppendLine($"---#{index}");                                        //   ---#1
            //            sb.AppendLine($"---SongName: {s.Name}");                              //   ---SongName: Numb
            //            sb.AppendLine($"---Price: {s.Price:F2}");                            //   --- Price: 13.99
            //            sb.AppendLine($"---Writer: {s.Writer.Name}");                        //   --- Writer: Kara - lynn Sharpous
            //            index++;
            //        }
            //        sb.AppendLine($"-AlbumPrice: {a.Price:F2}");
            //    }
            //}
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .AsEnumerable() // Прехвърляме заявката в паметта
                .Where(s => s.Duration.TotalSeconds > duration) // Филтрираме след зареждането
                .Select(s => new
                {
                    SongName = s.Name,
                    WriterName = s.Writer.Name,
                    Performers = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(p => p)
                        .ToList(),
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToList();

            int index = 1;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{index}")
                  .AppendLine($"---SongName: {song.SongName}")
                  .AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.Performers)
                {
                    sb.AppendLine($"---Performer: {performer}");
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                  .AppendLine($"---Duration: {song.Duration}");

                index++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}
