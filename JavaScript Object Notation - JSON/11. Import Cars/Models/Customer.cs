using System.ComponentModel.DataAnnotations;

namespace CarDealer.Models
{
    public class Customer
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual DateTime BirthDate { get; set; }

        public bool IsYoungDriver { get; set; }

        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>(); 
    }
}