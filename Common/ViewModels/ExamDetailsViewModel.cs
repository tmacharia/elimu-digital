using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ExamDetailsViewModel
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime Moment { get; set; }

        public ExamUnit Unit { get; set; }
        public ExamCourse Course { get; set; }
        public Profile Instructor { get; set; }

        public ICollection<ExamQuestion> Questions { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
    public class ExamQuestion
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public decimal Marks { get; set; }

        public ICollection<QuestionAnswer> Answers { get; set; }
    }
    public class QuestionAnswer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class ExamCourse
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Name { get; set; }
        public CourseType Type { get; set; }
    }
    public class ExamUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
