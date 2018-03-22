using Common.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IExamManager
    {
        IEnumerable<Exam> All { get; }
        IEnumerable<Exam> MyExams(int accountId);
        ExamDetailsViewModel Get(int id);
    }
}
