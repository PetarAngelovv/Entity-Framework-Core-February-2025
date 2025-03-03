
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace P01_StudentSystem.Data.Models
{
    public class Student
    {

        public Student()
        {
            StudentsCourses = new HashSet<StudentCourse>();
            Homeworks = new HashSet<Homework>();
        }
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Unicode(false)]
        [StringLength(10, MinimumLength = 10)]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> StudentsCourses { get; set; }
        public ICollection<Homework> Homeworks { get; set; }



    }
}
