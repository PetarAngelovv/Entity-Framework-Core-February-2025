using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static MusicHub.Common.GlobalConstants;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            PerformerSongs = new HashSet<SongPerformer>();
        }

        [Key]
        public int Id { get; set; } // Id – integer, Primary Key

        [MaxLength(PerformerNameMaxLength)]
        public string FirstName { get; set; } // FirstName – text with max length 20 (required) 


        [MaxLength(PerformerNameMaxLength)]
        public string LastName { get; set; } //	LastName – text with max length 20 (required) 

        [Required]
        public int Age { get; set; } //	Age – integer(required)

        [Required]
        public decimal NetWorth { get; set; } // NetWorth – decimal (required)

        public virtual ICollection<SongPerformer> PerformerSongs { get; set; } // PerformerSongs – a collection of type SongPerformer

    }
}
