using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DAL.Models;
using System.Reflection;
using Common.ViewModels;
using System.Collections;
using Services.Comparers;

namespace Services
{
    public class DataManager : IDataManager
    {
        private readonly IRepositoryFactory _repos;

        public DataManager(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        public IEnumerable<Lecturer> MyLecturers(int studentId, int count=5000)
        {
            // get all my units
            List<Lecturer> lecturers = _repos.StudentUnits
                                             .ListWith("Unit",
                                                        "Unit.Lecturer",
                                                        "Unit.Lecturer.Profile",
                                                        "Unit.Lecturer.Likes")
                                              .Where(x => x.StudentId == studentId)
                                              .Select(x => x.Unit)
                                              .Select(x => x.Lecturer)
                                              .Distinct()
                                              .ToList();

            // get lecturers by course

            var student = _repos.Students
                                .GetWith(studentId,
                                          "Course",
                                          "Course.Units",
                                          "Course.Units.Lecturer",
                                          "Course.Units.Lecturer.Profile",
                                          "Course.Units.Lecturer.Likes");
                              
            if(student != null)
            {
                if(student.Course != null)
                {
                    var lecs = student.Course.Units
                                      .SkipWhile(x => x.Lecturer == null)
                                      .Select(x => x.Lecturer)
                                      .Distinct()
                                      .ToList();

                    lecturers.AddRange(lecs);
                }
            }

            return lecturers.Distinct();
        }

        public IList<Student> MyStudents(int lecturerId, int count=5000)
        {
            IList<Student> students = _repos.Lecturers
                                            .GetWith(lecturerId, "Units",
                                                      "Units.UnitStudents",
                                                      "Units.UnitStudents.Student",
                                                      "Units.UnitStudents.Student.Course",
                                                      "Units.UnitStudents.Student.Profile")
                                            .Units
                                            .SelectMany(x => x.UnitStudents)
                                            .TakeWhile(x => x.Student != null)
                                            .Select(x => x.Student)
                                            .Take(count)
                                            .Distinct()
                                            .ToList();
            return students;
        }

        public IEnumerable<ClassUnitViewModel> MyClasses<T>(int id, int count=5000) where T : class
        {
            List<ClassUnitViewModel> classes = new List<ClassUnitViewModel>();

            if(id > 0)
            {
                if (typeof(T) == typeof(Student))
                {
                    var student = _repos.Students
                                        .GetWith(id,
                                                "StudentUnits",
                                                "StudentUnits.Unit",
                                                "StudentUnits.Unit.Class",
                                                "StudentUnits.Unit.Class.Units",
                                                "StudentUnits.Unit.Class.Likes");
                                    

                    if (student != null)
                    {
                        if(student.StudentUnits != null)
                        {
                            var _cl = student.StudentUnits
                                             .Select(x => x.Unit);

                            classes.AddRange(_cl.Select(Predicates.UnitToClass()));
                        }
                    }

                    var _cls = _repos.Students
                                     .GetWith(id,
                                      "Course",
                                      "Course.Units",
                                      "Course.Units.Class",
                                      "Course.Units.Class.Units",
                                      "Course.Units.Class.Likes");

                    if (_cls != null)
                    {
                        if(_cls.Course != null)
                        {
                            classes.AddRange(_cls.Course.Units.Select(Predicates.UnitToClass()));
                        }
                    }
                }
                else if (typeof(T) == typeof(Lecturer))
                {
                    var _cl = _repos.Lecturers
                                    .GetWith(id,
                                            "Units",
                                            "Units.Class",
                                            "Units.Class.Units",
                                            "Units.Class.Likes");

                    if (_cl != null)
                    {
                        classes.AddRange(_cl.Units.Select(Predicates.UnitToClass()));
                    }
                }
            }
            else
            {
                return classes;
            }

            return classes.Where(x => x != null)
                          .Distinct(new ClassUnitEqualityComparer())
                          .Take(count);
        }
        public MyCoursesViewModel MyCourses(int studentId)
        {
            MyCoursesViewModel model = new MyCoursesViewModel
            {
                Others = _repos.StudentCourses
                                 .ListWith("Course","Course.Units")
                                 .Where(x => x.StudentId == studentId)
                                 .Select(x => x.Course)
                                 .ToList()
            };

            var student = _repos.Students
                             .GetWith(studentId, "Course", "Course.Units");

            if(student != null)
            {
                if(student.Course != null)
                {
                    model.Main = student.Course;
                }
            }

            return model;
        }
        public IEnumerable<Unit> MyUnits<T>(int id, int count=5000) where T : class
        {
            List<Unit> units = new List<Unit>();

            if (typeof(T) == typeof(Student))
            {
                var std = _repos.Students
                                .GetWith(id,
                                         "Course",
                                         "Course.Units",
                                         "Course.Units.Lecturer",
                                         "Course.Units.Lecturer.Profile",
                                         "Course.Units.Class",
                                         "Course.Units.UnitStudents",
                                         "Course.Units.Likes",
                                         "Course.Units.Boards",
                                         "Course.Units.Boards.Unit",
                                         "Course.Units.Boards.Posts");

                if(std != null)
                {
                    if (std.Course != null)
                    {
                        units.AddRange(std.Course.Units);
                    }
                }

                var unitsInOtherCourses = _repos.StudentCourses
                                                .ListWith("Course",
                                                          "Course.Units",
                                                          "Course.Units.Lecturer",
                                                          "Course.Units.Lecturer.Profile",
                                                          "Course.Units.Class",
                                                          "Course.Units.Likes",
                                                          "Course.Units.UnitStudents",
                                                          "Course.Units.Likes",
                                                          "Course.Units.Boards",
                                                          "Course.Units.Boards.Unit",
                                                          "Course.Units.Boards.Posts")
                                                .Where(x => x.StudentId == id)
                                                .TakeWhile(x => x.Course != null)
                                                .Select(x => x.Course)
                                                .TakeWhile(x => x.Units != null && x.Units.Count > 0)
                                                .SelectMany(x => x.Units);
                                                

                if (unitsInOtherCourses != null)
                {
                    units.AddRange(unitsInOtherCourses);
                }
            }
            else if(typeof(T) == typeof(Lecturer))
            {
                var lec = _repos.Lecturers
                                .GetWith(id,
                                       "Units",
                                       "Units.Course",
                                       "Units.Class",
                                       "Units.Lecturer",
                                       "Units.Lecturer.Profile",
                                       "Units.UnitStudents",
                                       "Units.Likes",
                                       "Units.Boards",
                                       "Units.Boards.Unit",
                                       "Units.Boards.Posts");

                if(lec.Units != null)
                {
                    units.AddRange(lec.Units);
                }
            }

            return units.Distinct().Take(count);
        }

        public SummaryViewModel GetSummary()
        {
            // create summary view model
            var summary = new SummaryViewModel
            {
                Total_Classes = _repos.Classes.List.Count(),
                Students_Total = _repos.Students.List.Count(),
                Lec_Total = _repos.Lecturers.List.Count(),
                Courses_Total = _repos.Courses.List.Count(),
                Lec_NoProfile = _repos.Lecturers.ListWith("Profile")
                                                .Count(x => x.Profile == null),
                Students_Enrolled = _repos.Students.ListWith("Course")
                                                   .Count(x => x.Course != null),
                Units_NoClass = _repos.Units.ListWith("Class")
                                            .Count(x => x.Class == null)
            };

            return summary;
        }

        public IEnumerable<Student> MyClassMates(int id)
        {
            IList<Student> students = new List<Student>();

            students = _repos.Students
                             .GetWith(id,
                                    "StudentUnits",
                                    "StudentUnits.Student",
                                    "StudentUnits.Student.Profile",
                                    "StudentUnits.Student.Course")
                             ?.StudentUnits
                             .Select(x => x.Student)
                             .SkipWhile(x => x == null || x.Id == id)
                             .Distinct()
                             .ToList();

            var courseStudents = _repos.StudentCourses
                                       .ListWith("Student", "Student.Profile")
                                       .Where(x => x.CourseId == id)
                                       .Select(x => x.Student)
                                       .ToList();

            foreach (var item in courseStudents)
            {
                students.Add(item);
            }

            if (students == null)
            {
                return new List<Student>();
            }
            else
            {
                return students.Distinct();
            }
        }

        public IEnumerable<Lecturer> MyColleagues(int lecturerId)
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            //var lecturer = _repos.Lecturers
            //                     .ListWith("Units", "Units.Course")
            //                     .FirstOrDefault(x => x.Id == lecturerId);

            //var courses = lecturer.Units
            //                      .Select(x => x.Course)
            //                      .Distinct()
            //                      .ToList();

            lecturers = _repos.Lecturers.ListWith("Units",
                                                  "Units.Course",
                                                  "Units.Course.Units",
                                                  "Units.Course.Units.Lecturer",
                                                  "Units.Course.Units.Lecturer.Profile")
                                        .Where(x => x.Id == lecturerId)
                                        .SelectMany(x => x.Units)
                                        .Select(x => x.Course)
                                        .SelectMany(x => x.Units)
                                        .Select(x => x.Lecturer)
                                        .TakeWhile(x => x != null)
                                        .ToList();

            //foreach (var item in courses)
            //{
            //    var lecs = _repos.Units
            //                     .ListWith("Course",
            //                               "Lecturer",
            //                               "Lecturer.Profile")
            //                     .Where(x => x.Course.Id == item.Id)
            //                     .Select(x => x.Lecturer)
            //                     .ToList();

            //    lecturers.AddRange(lecs);
            //}

            return lecturers.TakeWhile(x => x != null)
                            .TakeWhile(x => x.Id != lecturerId)
                            .Distinct()
                            .ToList();
        }

        public IList<ExamViewModel> MyExams<T>(int id, int count = 5000) where T : class
        {
            List<ExamViewModel> exams = new List<ExamViewModel>();

            if (typeof(T) == typeof(Student))
            {
                var std = _repos.Students
                                .GetWith(id,
                                       "Course",
                                       "Course.Units",
                                       "Course.Units.Exams",
                                       "Course.Units.Exams.Unit");

                if (std != null)
                {
                    if (std.Course != null)
                    {
                        exams.AddRange(std.Course.Units
                                          .SelectMany(x => x.Exams)
                                          .Select(TransformFuncs.ToViewModel()));
                    }
                }

                var _otherExams = _repos.StudentCourses
                                        .ListWith("Course",
                                                  "Course.Units",
                                                  "Course.Units.Exams",
                                                  "Course.Units.Exams.Unit")
                                        .Where(x => x.StudentId == id)
                                        .Select(x => x.Course)
                                        .SelectMany(x => x.Units)
                                        .SelectMany(x => x.Exams)
                                        .Select(TransformFuncs.ToViewModel());


                if (_otherExams != null)
                {
                    exams.AddRange(_otherExams);
                }
            }
            else if (typeof(T) == typeof(Lecturer))
            {
                var lec = _repos.Lecturers
                                .GetWith(id,
                                       "Units",
                                       "Units.Exams",
                                       "Units.Exams.Unit");

                if (lec.Units != null)
                {
                    var _exams = lec.Units.SelectMany(x => x.Exams)
                                          .Select(TransformFuncs.ToViewModel());

                    exams.AddRange(_exams);
                }
            }

            return exams.Take(count)
                        .ToList();
        }

        public ExamDetailsViewModel GetExam(int id)
        {
            Exam exam = _repos.Exams.GetWith(id, "Unit",
                                     "Unit.Course",
                                     "Unit.Lecturer",
                                     "Unit.Lecturer.Profile",
                                     "Questions",
                                     "Questions.Answers",
                                     "Likes",
                                     "Comments");


            if (exam == null)
            {
                throw new Exception("Exam with that id does not exist in records.");
            }

            var model = new ExamDetailsViewModel()
            {
                Code = exam.Code,
                Comments = exam.Comments,
                Date = exam.Date,
                End = exam.End,
                Id = exam.Id,
                Instructor = exam.Unit.Lecturer.Profile,
                Unit = new ExamUnit()
                {
                    Id = exam.Unit.Id,
                    Name = exam.Unit.Name
                },
                Course = new ExamCourse()
                {
                    Id = exam.Unit.Course.Id,
                    Code = exam.Unit.Course.Code,
                    Name = exam.Unit.Course.Name,
                    Type = exam.Unit.Course.Type
                },
                Name = exam.Name,
                Likes = exam.Likes,
                Start = exam.Start,
                Moment = exam.Moment,
                Questions = exam.Questions.Select(q => new ExamQuestion()
                {
                    Id = q.Id,
                    Marks = q.Marks,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new QuestionAnswer()
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };

            return model;
        }

        public IList<DiscussionBoard> MyBoards<T>(int id, int count = 5000) where T : class
        {
            return MyUnits<T>(id, count).SelectMany(x => x.Boards)
                                        .ToList();
        }

        public IList<ParticipantViewModel> GetBoardParticipant(int id, int count = 5000)
        {
            List<ParticipantViewModel> list = new List<ParticipantViewModel>();

            var board = _repos.DiscussionBoards
                              .GetWith(id,
                              "Unit",
                              "Unit.Lecturer",
                              "Unit.Lecturer.Profile",
                              "Unit.UnitStudents",
                              "Unit.UnitStudents.Student",
                              "Unit.UnitStudents.Student.Profile");

            if(board == null)
            {
                return list;
            }
            else
            {
                if(board.Unit.Lecturer != null)
                {
                    var lec = new ParticipantViewModel(AccountType.Lecturer)
                    {
                        AccountId = board.Unit.Lecturer.Id,
                        Names = board.Unit.Lecturer.Profile.FullNames,
                        Photo = board.Unit.Lecturer.Profile.PhotoUrl
                    };

                    list.Add(lec);
                }

                var others = board.Unit
                                  .UnitStudents
                                  .Select(x => new ParticipantViewModel(AccountType.Student)
                                  {
                                      AccountId = x.StudentId,
                                      Names = x.Student.Profile.FullNames,
                                      Photo = x.Student.Profile.PhotoUrl
                                  }).ToList();

                list.AddRange(others);
            }

            return list.Take(count).ToList();
        }

        
    }
}
