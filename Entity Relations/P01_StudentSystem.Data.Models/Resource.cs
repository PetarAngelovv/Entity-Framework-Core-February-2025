using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
     
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Unicode(false)]
        [Required]
        public string Url { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; } 
        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
    public enum ResourceType
    {
        Video,
        Presentation,
        Document,
        Other
    }
}
