using DAL.Contexts;
using DAL.Factories;
using DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Common;
using Newtonsoft.Json;
using System.Diagnostics;
using Services;
using DAL.Extensions;
using DAL.Attributes;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ConsoleTest
{
    class Program
    {
        private static LePadContext _context = new LePadContextFactory().Create();
        private static AppDbContext _appDb = new AppDbContextFactory().Create();
        private static IRepositoryFactory _repos = new RepositoryFactory(_context);
        private static Stopwatch _syswatch = new Stopwatch();
        private static Stopwatch _sqlwatch = new Stopwatch();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Hello E-Learning Pad!\n\n====================\n\n");

            Console.ForegroundColor = ConsoleColor.Green;

            //Entry();
            //ConfirmDate();
            //DayHandler();
            SeedProfileIdClaims();
            //InitializeGet<School>();
            //SeedSchool("Online");
            //Get<School>(1, "Location");

            Console.ReadKey();
        }

        static void SeedRoles()
        {
            
        }
        static void SeedProfileIdClaims()
        {
            IList<AppUser> users = new List<AppUser>();
            List<IdentityUserClaim<string>> identityClaims = new List<IdentityUserClaim<string>>();

            Console.WriteLine("Getting users...");
            users = _appDb.Users.ToList();
            Console.WriteLine($"{users.Count} users found.");

            foreach (var user in users)
            {
                IdentityUserClaim<string> claim = new IdentityUserClaim<string>
                {
                    ClaimType = "ProfileId",
                    UserId = user.Id
                };

                Console.WriteLine($"User: {user.Id} is a {user.AccountType.ToString()}");

                if (user.AccountType == AccountType.Student)
                {
                    var student = _context.Students
                                          .Include(x => x.Profile)
                                          .FirstOrDefault(x => x.Id == user.AccountId);

                    claim.ClaimValue = student?.Profile?.Id.ToString();
                }
                else if (user.AccountType == AccountType.Lecturer)
                {
                    var lecture = _context.Lecturers
                                          .Include(x => x.Profile)
                                          .FirstOrDefault(x => x.Id == user.AccountId);

                    claim.ClaimValue = lecture?.Profile?.Id.ToString();
                }
                else if (user.AccountType == AccountType.Administrator)
                {
                    var adminis = _context.Administrators
                                          .Include(x => x.Profile)
                                          .FirstOrDefault(x => x.Id == user.AccountId);

                    claim.ClaimValue = adminis?.Profile?.Id.ToString();
                }

                Console.WriteLine("Adding user claim to memory.");
                identityClaims.Add(claim);
            }
            Console.WriteLine("Finished creating claims.");

            _appDb.UserClaims.AddRange(identityClaims);

            Console.WriteLine("Saving to database...");
            _appDb.SaveChanges();
            Console.WriteLine("\n\nOperation completed successfully!");
        }
        static void SeedSchool(string db)
        {
            _syswatch.Start();
            _sqlwatch.Start();

            Console.WriteLine($"Adding default uni/school to the {db} database...");

            School school = new School()
            {
                DateFounded = new DateTime(1949, 10, 5),
                Location = new Location()
                {
                    City = "Nairobi",
                    Street = "Ngong Road, Upper Hill",
                    Latitude = -1.299719,
                    Longitude = 36.816097
                },
                Name = "GreenStalk University",
                ViceChancellor = "Dr. Benard Masinde",
                Website = "https://www.greenstalk.uni",
            };

            Console.WriteLine("Commiting changes...");
            school = _context.Schools.Add(school).Entity;
            _context.SaveChanges();

            Console.WriteLine("DONE!!");

            PrintTimeLapse();

            Get<School>(school.Id, "Location");
        }
        static void DayHandler()
        {
            //days ago
            DateTime d1 = new DateTime(2018, 2, 15);
            Console.WriteLine("Date 1:{0}, Total Days:{1}",d1.ToString(),d1.DayHandler());
            //3 days ago
            DateTime d2 = new DateTime(2018, 2, 28);
            Console.WriteLine("Date 2:{0}, Total Days:{1}", d2.ToString(), d2.DayHandler());
            //1 day ago
            DateTime d3 = new DateTime(2018, 3, 1);
            Console.WriteLine("Date 3:{0}, Total Days:{1}", d3.ToString(), d3.DayHandler());
            //today
            DateTime d4 = new DateTime(2018, 3, 2);
            Console.WriteLine("Date 4:{0}, Total Days:{1}", d4.ToString(), d4.DayHandler());
            //tomorrow
            DateTime d5 = new DateTime(2018, 3, 4);
            Console.WriteLine("Date 5:{0}, Total Days:{1}", d5.ToString(), d5.DayHandler());
            //in couple of days
            DateTime d6 = new DateTime(2018, 3, 24);
            Console.WriteLine("Date 6:{0}, Total Days:{1}", d6.ToString(), d6.DayHandler());
        }
        //static void SeedCourses()
        static void ConfirmDate()
        {
            Console.WriteLine("Datetime now: {0}", DateTime.Now.ToString());
            Console.WriteLine("Time Part: {0}", DateTime.Now.ToString("tt"));

            Console.WriteLine("\n\nTimeSpan: {0}", DateTime.Now.TimeOfDay.ToString());

            ValidateStart(DateTime.Now.Subtract(new TimeSpan(12,0,0)));
        }
        private static bool ValidateStart(DateTime start)
        {
            TimeSpan span = start.TimeOfDay;
            List<string> errors = new List<string>();

            // not below 7:00 A.M
            if (start.Meridiem() == Meridiem.AM)
            {
                if(span.Hours < 7)
                {
                    errors.Add("University policy requires all classes to begin at or after 7:00 A.M");
                    return false;
                }
            }

            // not greater than 7:00 P.M
            if (start.Meridiem() == Meridiem.PM)
            {
                if(span.Hours > 7)
                {
                    errors.Add("A class can only take a minimum of 2 hours and cannot go beyond 9:00 P.M." +
                        $"Starting at {span.Hours}:{span.Minutes} {start.Meridiem().ToString()} does not follow University class policy rules.");
                    return false;
                }
            }

            return true;
        }
        private static bool ValidateStop(DateTime stop)
        {
            return false;
        }
        static void InitializeGet<TEntity>() where TEntity : class
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Specify Id of the {typeof(TEntity).Name} you want to get: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string num = Console.ReadLine();

            int i = int.Parse(num);

            if(i < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Id. Should be greater than 0");
                InitializeGet<TEntity>();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Navigation properties to include(comma separated.): ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;

            if (string.IsNullOrWhiteSpace(input))
            {
                Get<TEntity>(i);
            }
            else
            {
                string[] tokens = input.Split(',');

                if(tokens.Length > 0)
                {
                    Get<TEntity>(i, tokens);
                }
            }
            
        }
        static void Get<TEntity>(int id, params string[] navigationProps) where TEntity : class
        {
            _syswatch.Start();
            Console.WriteLine($"Fetching {typeof(TEntity).Name} with 'Id' {id}...\n");
            _sqlwatch.Start();

            TEntity entity = _context.Set<TEntity>()
                                     .IncludeWith(navigationProps)
                                     .FirstOrDefault(x => (int)x
                                     .GetType()
                                     .GetProperty("Id")
                                     .GetValue(x) == id);
            _sqlwatch.Stop();

            if(entity == null)
            {
                Console.WriteLine($"{typeof(TEntity).Name} with 'Id' {id} not found.");
            }
            else
            {
                Log(entity);
            }
        }
        static void Log<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyInfo[] props = typeof(TEntity).GetProperties()
                                                  .OrderBy(x => x.Name)
                                                  .ToArray();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];

                object val = prop.GetValue(entity);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(prop.Name + " : ");

                if (val is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"null\n");
                }
                else if (val is Base)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(GetJson(val));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write($"{val.ToString()}\n");
                }
            }

            PrintTimeLapse();
        }
        static void PrintTimeLapse()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n+++++++++++++++++++");
            Console.WriteLine($"SQL timelapse: {_sqlwatch.Elapsed.Minutes}min, {_sqlwatch.Elapsed.Seconds}sec, {_sqlwatch.Elapsed.Milliseconds}ms\n");
            _syswatch.Stop();
            Console.WriteLine($"System timelapse: {_syswatch.Elapsed.Minutes}min, {_syswatch.Elapsed.Seconds}sec, {_syswatch.Elapsed.Milliseconds}ms");
            Console.WriteLine("===================\n\n");
            _sqlwatch.Reset();
            _syswatch.Reset();
        }
        static string GetJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented) + "\n";
        }
        static void Save<T>(T entity) where T : class
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Saving to database...");
            _context.Set<T>()
                    .Add(entity);

            _context.SaveChanges();

            Console.WriteLine("Saved!!");
        }
        static void AddSchool()
        {
            Console.WriteLine("Create new school. -->\n");

            School school = new School();

            school = RequestConsole<School>();
            school.Location = RequestConsole<Location>();

            Save(school);
        }
        static T RequestConsole<T>() where T : class
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Specify {typeof(T).Name} details\n");

            T entity = (T)Activator.CreateInstance<T>();

            string[] ignore = new string[] { "Id", "Timestamp" };

            PropertyInfo[] props = typeof(T).GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];

                if (ignore.Contains(prop.Name))
                    break;

                if(prop.GetType() == typeof(int))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{prop.Name}: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    int num = int.Parse(Console.ReadLine());

                    prop.SetValue(entity, num);
                }
                else if(prop.GetType() == typeof(decimal))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{prop.Name}: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    decimal num = decimal.Parse(Console.ReadLine());

                    prop.SetValue(entity, num);
                }
                else if(prop.GetType() == typeof(string))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{prop.Name}: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string input = Console.ReadLine();

                    prop.SetValue(entity, input);
                }
                else if (prop.GetType().IsArray)
                {
                    Console.WriteLine("Skipping array type.\n");
                }
                else if(prop.GetType() == typeof(Enum))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{prop.Name}: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    int num = int.Parse(Console.ReadLine());

                    prop.SetValue(entity, num);
                }
                else if (prop.GetType() == typeof(Guid))
                {
                    prop.SetValue(entity, Guid.NewGuid());
                }
            }

            return entity;
        }
        static void AddCourse()
        {
            Console.WriteLine("Create new course. -->\n");

            Console.WriteLine("To which school do you want to add the new course to? Specify Id");

            int num = int.Parse(Console.ReadLine());

            Course course = new Course();

            course = RequestConsole<Course>();

            School school = _context.Schools.FirstOrDefault(x => x.Id == num);

            if(school == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("School with that Id does not exist.");
                Console.ForegroundColor = ConsoleColor.Green;

                Entry();
            }
            else
            {
                course.School = school;
            }

            Save(course);
        }
        static void List<T>() where T : class
        {
            Console.WriteLine($"Listing all {typeof(T).Name}'s");

            var list = _context.Set<T>().ToList();

            foreach (var item in list)
            {
                Log(item);
            }
        }
        static void ListCoursesInSchool(int id)
        {
            var courses = _repos.Courses
                                .ListWith("School")
                                .Where(x => x.School.Id == id)
                                .ToList();

            foreach (var item in courses)
            {
                Log(item);
            }
        }
        static void AddUnit()
        {
            Console.WriteLine("Create new Unit. -->\n");

            Console.WriteLine("To which course do you want to add the new unit to? Specify Id");

            int num = int.Parse(Console.ReadLine());

            Unit unit = new Unit();

            unit = RequestConsole<Unit>();

            Course course = _context.Courses.FirstOrDefault(x => x.Id == num);

            if (course == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Course with that Id does not exist.");
                Console.ForegroundColor = ConsoleColor.Green;

                Entry();
            }
            else
            {
                unit.Course = course;
            }

            Save(unit);
        }
        static void Entry()
        {
            while (true)
            {
                Console.WriteLine("Root menu :\n\n" +
                "1. Add school/university.\n" +
                "2. List schools\n" +
                "3. Add course to school\n" +
                "4. View courses in school\n" +
                "5. Add unit to course\n" +
                "6. View units in course\n\n" +
                "ESC: press 'q' to quit.");

                string input = Console.ReadLine();

                if (input == "q" || input == "Q")
                {
                    Environment.Exit(1);
                }

                try
                {
                    int num = int.Parse(input);

                    if (num < 1 || num > 6)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid list item.");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Entry();
                    }
                    else
                    {
                        switch (num)
                        {
                            case 1:
                                AddSchool();
                                break;
                            case 2:
                                List<School>();
                                break;
                            case 3:
                                AddCourse();
                                break;
                            case 4:
                                ListCoursesInSchool(1);
                                break;
                            case 5:
                                AddUnit();
                                break;
                            case 6:
                                List<Unit>();
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong value entered. Only enter numbers represeting actions to perform in the list menu.");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Entry();
                }
            }
        }
    }
}
