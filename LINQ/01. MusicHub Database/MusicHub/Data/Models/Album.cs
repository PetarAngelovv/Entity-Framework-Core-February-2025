using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MusicHub.Common.GlobalConstants;
namespace MusicHub.Data.Models
{
    public class Album
    {
        public Album()
        {
            Songs = new HashSet<Song>();
        }
        [Key]
        public int Id { get; set; } // Id – integer, Primary Key

        [Required]
        [MaxLength(AlbumNameMaxLength)]
        public string Name { get; set; } //	Name – text with max length 40 (required)

        [Required]
        public DateTime ReleaseDate { get; set; } //	ReleaseDate – date(required)

        public decimal Price => Songs.Sum(s => s.Price); // Price – calculated property(the sum of all song prices in the album)

        [ForeignKey(nameof(Producer))]
        public int? ProducerId { get; set; } //	ProducerId – integer, foreign key
        public Producer Producer { get; set; } // Producer – the Album's Producer

        public virtual ICollection<Song> Songs { get; set; } //	Songs – a collection of all Songs in the Album

    }
}
