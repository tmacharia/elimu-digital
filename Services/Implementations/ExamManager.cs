using System;
using System.Collections.Generic;
using System.Text;
using Common.ViewModels;
using DAL.Models;

namespace Services
{
    public class ExamManager : IExamManager
    {
        private readonly IRepositoryFactory _repos;
        public ExamManager(IRepositoryFactory factory)
        {
            _repos = factory;
        }
        public IEnumerable<Exam> All => _repos.Exams.List;

        public ExamDetailsViewModel Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Exam> MyExams(int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
