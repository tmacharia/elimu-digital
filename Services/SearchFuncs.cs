using DAL.Models;
using System;
using Services;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    public static class SearchFuncs
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
            string pattern = $"({query})";

            bool func(Course item)
            {
                // search by course name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern)) return true;
                    else return false;
                }

                return false;
            };

            return func;
        }
        public static Func<Unit, bool> Unit(string query)
        {
            string pattern = $"({query})";

            bool func(Unit item)
            {
                // search by unit name
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    if (Regex.IsMatch(item.Name, pattern)) return true;
                    else return false;
                }

                return false;
            };

            return func;
        }

    }
}
