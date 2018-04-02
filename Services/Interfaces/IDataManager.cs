using Common.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IDataManager
    {
        SummaryViewModel GetSummary();
        IList<Student> MyStudents(int lecturerId,int count);
        IEnumerable<Lecturer> MyLecturers(int studentId, int count=5000);
        IEnumerable<Lecturer> MyColleagues(int lecturerId);
        MyCoursesViewModel MyCourses(int studentId);
        ExamDetailsViewModel GetExam(int id);

        IEnumerable<Student> MyClassMates(int id);
        IEnumerable<Unit> MyUnits<T>(int id, int count=5000) where T : class;
        IEnumerable<Class> MyClasses<T>(int id, int count=5000) where T : class;
        IList<ExamViewModel> MyExams<T>(int id, int count = 5000) where T : class;
    }
}
