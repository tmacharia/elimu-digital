using DAL.Models;
using System;
using Services;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Common.ViewModels;

namespace Services
{
    public static class Predicates
    {
        public static Func<School, bool> School(string query)
        {
            string pattern = $"({query})";

            bool func(School item)
            {
                // search by school name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern)) return true;
                    else return false;
                }

                // search by vice chancellor's name
                if (!string.IsNullOrWhiteSpace(item.ViceChancellor))
                {
                    if (Regex.IsMatch(item.ViceChancellor, pattern)) return true;
                    else return false;
                }

                // search by location of school
                if (item.Location != null)
                {
                    if (Regex.IsMatch(item.Location.City, pattern)) return true;
                    else return false;
                }

                // search by courses offered 
                if (item.Courses != null && item.Courses.Count > 0)
                {
                    if (item.Courses.Contains(x => x.Name != null && Regex.IsMatch(x.Name, pattern)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            };

            return func;
        }
        public static Func<Course, bool> Course(string query)
        {
            string pattern = "(" + query + ")";

            bool func(Course item)
            {
                // search by course name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern, RegexOptions.IgnoreCase)) return true;
                    else return false;
                }

                return false;
            };

            return func;
        }
        public static Func<Lecturer, bool> Lecturer(string query)
        {
            string pattern = "(" + query + ")";

            bool func(Lecturer item)
            {
                // search by course name
                if (!string.IsNullOrWhiteSpace(item.Profile.FullNames))
                {
                    if (Regex.IsMatch(item.Profile.FullNames, pattern, RegexOptions.IgnoreCase)) return true;
                    else return false;
                }

                return false;
            };

            return func;
        }
        public static Func<Student, bool> Student(string query)
        {
            string pattern = "(" + query + ")";

            bool func(Student item)
            {
                // search by course name
                if (!string.IsNullOrWhiteSpace(item.Profile.FullNames))
                {
                    if (Regex.IsMatch(item.Profile.FullNames, pattern, RegexOptions.IgnoreCase)) return true;
                    else return false;
                }

                return false;
            };

            return func;
        }
        public static Func<DiscussionBoard, bool> Boards(string query)
        {
            string pattern = "(" + query + ")";

            bool func(DiscussionBoard item)
            {
                // search by course name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(item.Unit.Name, pattern, RegexOptions.IgnoreCase))
                        return true;
                    else
                        return false;
                }

                return false;
            };

            return func;
        }

        public static Func<Unit, bool> Unit(string query)
        {
            string pattern = "(" + query + ")";

            bool func(Unit item)
            {
                // search by unit name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern))
                        return true;
                    else
                        return false;
                }

                return false;
            };

            return func;
        }
        public static Func<Class, ClassUnitViewModel> ToClass()
        {
            ClassUnitViewModel func(Class @class)
            {
                if(@class != null)
                {
                    return new ClassUnitViewModel()
                    {
                        DayOfWeek = @class.DayOfWeek,
                        EndTime = @class.EndTime,
                        Likes = @class.Likes,
                        Room = @class.Room,
                        StartTime = @class.StartTime,
                        Units = @class.Units,
                        Unit = string.Empty,
                        ClassId = @class.Id
                    };
                }
                else { return null; }
            }

            return func;
        }
        public static Func<Unit, ClassUnitViewModel> UnitToClass()
        {
            ClassUnitViewModel func(Unit unit)
            {
                // check if unit is null
                if(unit != null)
                {
                    // check if class is null
                    if(unit.Class != null)
                    {
                        return new ClassUnitViewModel()
                        {
                            DayOfWeek = unit.Class.DayOfWeek,
                            EndTime = unit.Class.EndTime,
                            Likes = unit.Class.Likes,
                            Room = unit.Class.Room,
                            StartTime = unit.Class.StartTime,
                            Units = unit.Class.Units,
                            Unit = unit.Name,
                            UnitId = unit.Id,
                            ClassId = unit.Class.Id
                        };
                    }
                    else { return null; }
                }
                else { return null; }
                
            }
            return func;
        }
    }
}
