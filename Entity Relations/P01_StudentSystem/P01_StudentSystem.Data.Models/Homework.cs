using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
       
        public int HomeworkId { get; set; }

        [Required]
        [Unicode(false)]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }
        
        [Required]
        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }


        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
    public enum ContentType
    {
            Application,
            Pdf,
            Zip
    }
}
