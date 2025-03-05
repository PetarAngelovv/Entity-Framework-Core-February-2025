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
    public class Producer
    {
        public Producer()
        {
            Albums = new HashSet<Album>();
        }
        [Key]
        public int Id { get; set; } // Id – integer, Primary Key

        [Required]
        [MaxLength(ProducerNameMaxLength)]
        public string Name { get; set; } // Name – text with max length 30 (required)
        public string? Pseudonym { get; set; } // Pseudonym – text
        public string? PhoneNumber { get; set; } // PhoneNumber – text
        public virtual ICollection<Album> Albums { get; set; } // Albums – a collection of type Album

    }
}
