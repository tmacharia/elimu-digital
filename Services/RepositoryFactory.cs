using Common;
using DAL.Contexts;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class RepositoryFactory : IRepositoryFactory
    {
        #region Private Variables
        private readonly LePadContext _context;
        private readonly IRepository<Answer> _answers;
        private readonly IRepository<Comment> _comments;
        private readonly IRepository<Content> _contents;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<Like> _likes;
        private readonly IRepository<Location> _locations;
        private readonly IRepository<Profile> _profiles;
        private readonly IRepository<Question> _questions;
        private readonly IRepository<School> _schools;
        private readonly IRepository<Score> _scores;
        private readonly IRepository<Unit> _units;
        private readonly IRepository<Student> _students;
        private readonly IRepository<Lecturer> _lecturers;
        private readonly IRepository<Admin> _admins;
        private readonly IRepository<Class> _classes;
        private readonly IRepository<Notification> _notifications;
        #endregion

        public RepositoryFactory(LePadContext context)
        {
            _context = context;

            _answers = new Repository<Answer>(_context);
            _comments = new Repository<Comment>(_context);
            _contents = new Repository<Content>(_context);
            _courses = new Repository<Course>(_context);
            _exams = new Repository<Exam>(_context);
            _likes = new Repository<Like>(_context);
            _locations = new Repository<Location>(_context);
            _profiles = new Repository<Profile>(_context);
            _questions = new Repository<Question>(_context);
            _schools = new Repository<School>(_context);
            _scores = new Repository<Score>(_context);
            _units = new Repository<Unit>(_context);
            _students = new Repository<Student>(_context);
            _lecturers = new Repository<Lecturer>(_context);
            _admins = new Repository<Admin>(_context);
            _classes = new Repository<Class>(_context);
            _notifications = new Repository<Notification>(_context);
        }

        public IRepository<Answer> Answers => _answers;

        public IRepository<Comment> Comments => _comments;

        public IRepository<Content> Contents => _contents;

        public IRepository<Course> Courses => _courses;

        public IRepository<Exam> Exams => _exams;

        public IRepository<Like> Likes => _likes;

        public IRepository<Location> Locations => _locations;

        public IRepository<Profile> Profiles => _profiles;

        public IRepository<Question> Questions => _questions;

        public IRepository<School> Schools => _schools;

        public IRepository<Score> Scores => _scores;

        public IRepository<Unit> Units => _units;

        public IRepository<Student> Students => _students;

        public IRepository<Lecturer> Lecturers => _lecturers;

        public IRepository<Admin> Administrators => _admins;

        public IRepository<Class> Classes => _classes;

        public IRepository<Notification> Notifications => _notifications;

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
