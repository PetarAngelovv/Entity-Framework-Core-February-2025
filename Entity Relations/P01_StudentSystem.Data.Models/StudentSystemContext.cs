using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace P01_StudentSystem.Data.Models
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
           : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StudentCourse> StudentsCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
              optionsBuilder.UseSqlServer("Server=.;Database=Student_System;Trusted_Connection=True;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.CourseId, sc.StudentId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(sc => sc.StudentsCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(sc => sc.StudentsCourses)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<Resource>()
                .Property(r => r.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Homework>()
                .Property(h => h.Content)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
              .Property(s => s.PhoneNumber)
              .IsUnicode(false);
        }
       
    }
}
