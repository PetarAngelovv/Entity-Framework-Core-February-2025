namespace ProductShop.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public int? Age { get; set; }
   
        public virtual ICollection<Product> ProductsSold { get; set; } = new HashSet<Product>();

        public virtual ICollection<Product> ProductsBought { get; set; } = new HashSet<Product>();
    }
}