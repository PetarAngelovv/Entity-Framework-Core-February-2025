using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [ForeignKey(nameof(Song))]
        public int SongId { get; set; } // SongId – integer, Primary Key
        [Required]
        public Song Song { get; set; }// Song – the performer's Song (required)



        [ForeignKey(nameof(Performer))]
        public int PerformerId { get; set; } // PerformerId – integer, Primary Key
        [Required]
        public Performer Performer { get; set; }    // Performer – the Song's Performer (required)

    }
}
