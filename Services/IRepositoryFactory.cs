using Common;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IRepositoryFactory
    {
        IRepository<Answer> Answers { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Content> Contents { get; }
        IRepository<Course> Courses { get; }
        IRepository<Exam> Exams { get; }
        IRepository<Like> Likes { get; }
        IRepository<Location> Locations { get; }
        IRepository<Profile> Profiles { get; }
        IRepository<Question> Questions { get; }
        IRepository<School> Schools { get; }
        IRepository<Score> Scores { get; }
        IRepository<Unit> Units { get; }
        IRepository<Student> Students { get; }

        void Commit();
    }
}
