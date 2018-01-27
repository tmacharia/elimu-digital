using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Contexts
{
    public class LePadContext : DbContext
    {
        public LePadContext(DbContextOptions<LePadContext> options)
            :base(options) { }


        
        // Database Tables/DbSets
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Admin> Administrators { get; set; }
        public virtual DbSet<StudentUnit> StudentUnits { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentUnit>()
                   .HasKey(s => new { s.StudentId, s.UnitId });

            builder.Entity<StudentUnit>()
                   .HasOne(s => s.Student)
                   .WithMany(s => s.StudentUnits)
                   .HasForeignKey(s => s.StudentId);

            builder.Entity<StudentUnit>()
                   .HasOne(s => s.Unit)
                   .WithMany(s => s.UnitStudents)
                   .HasForeignKey(s => s.UnitId);
        }
    }
}
