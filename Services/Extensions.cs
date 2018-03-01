using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Common.ViewModels;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Services.Security;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Services
{
    /// <summary>
    /// A bunch of extension methods to use system wide.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds all mapping profiles for the application to the default Dependency Injection
        /// container for asp.net core as a singleton service. To use the added service in
        /// controllers, just inject <see cref="IMapper"/> into the constructor of your
        /// controller & continue with mapping.
        /// 
        /// </summary>
        /// <param name="services">Collection of services that get bootstrapped with the application
        /// into the DI container when the application fires up.
        /// </param>
        public static void AddAutoMapper(this IServiceCollection services)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMappingProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "E-learning Pad API",
                    Description = "e-learning application to support students & lecturers in campuses collaborate virtually on learning materials, unit registrations, exam scheduling & progress tracking.",
                    TermsOfService = "None",
                    Contact = new Contact()
                    {
                        Name = "Timothy Maina",
                        Email = "devtimmy@hotmail.com",
                        Url = "https://e-learningpad.com/"
                    },
                    License = new License()
                    {
                        Name = "GNU General Public License v3.0",
                        Url = "https://github.com/devTimmy/E-Learning-Pad/blob/master/LICENSE"
                    }
                });
            });
        }
        public static UpdateResult<TEntity> UpdateReflector<TEntity, TModel>(this TEntity entity, TModel model)
            where TEntity : class
            where TModel : class
        {
            UpdateResult<TEntity> result = new UpdateResult<TEntity>();

            // step through all properties
            PropertyInfo[] entity_props = typeof(TEntity).GetProperties();
            PropertyInfo[] model_props = typeof(TModel).GetProperties();

            for (int i = 0; i < model_props.Length; i++)
            {
                Console.WriteLine($"Looping through property: {model_props[i].Name}");

                if (model_props[i].Name == "Id")
                    break;

                var m_val = model_props[i].GetValue(model);
                var e_val = entity_props[i].GetValue(entity);

                if (e_val == null && m_val == null)
                    break;

                bool comparison = m_val.Equals(e_val);

                if(!comparison)
                {
                    entity_props[i].SetValue(entity, m_val);
                    Console.WriteLine($"Updating property --> '{entity_props[i].Name}' from {e_val} to {m_val}");
                    result.TotalUpdates++;
                }
            }

            result.Value = entity;
            return result;
        }
        public static bool Contains<T>(this ICollection<T> list, Func<T,bool> predicate)
        {
            if (list.Count(predicate) > 0)
                return true;
            else
                return false;
        }
        public static IApplicationBuilder UseSecurity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityMiddleware>();
        }
        public static string Trancate(this string s)
        {
            if(s.Length < 25)
            {
                return s;
            }
            else
            {
                return s.Substring(0, 24) + "...";
            }
        }
        public static string Uni
        {
            get
            {
                return "Greenstalk University";
            }
        }
        private static string RemoveAccent(string s)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(s);

            return Encoding.ASCII.GetString(bytes);
        }
        public static string GenerateSlug<T>(this T entity) where T : class
        {
            string name = typeof(T).GetProperty("Name").GetValue(entity).ToString();

            return Slugger(name);
        }
        public static string GenerateSlug(string s)
        {
            return Slugger(s);
        }
        private static string Slugger(string s)
        {
            string phrase = string.Format("{0}", s);

            string str = RemoveAccent(phrase).ToLower();

            // remove invalid characters
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // convert multiple spaces into single spaces
            str = Regex.Replace(str, @"\s+", " ").Trim();

            // cut and trim
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();

            // replace single-spaces with hyphens
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }
        public static string ToMoment(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            if(timeSpan.Days > 0)
            {
                return timeSpan.Days + " days ago";
            }
            else if(timeSpan.Hours > 0)
            {
                return timeSpan.Hours + "hrs ago";
            }
            else if(timeSpan.Minutes > 0)
            {
                return timeSpan.Minutes + "mins ago";
            }
            else if(timeSpan.Seconds > 0)
            {
                return timeSpan.Seconds + "secs ago";
            }

            return "ago";
        }
        public static IList<string> Populate(this ModelStateDictionary modelState)
        {
            List<string> list = new List<string>();

            foreach (var item in modelState.OrderBy(x => x.Key))
            {
                foreach (var it in item.Value.Errors)
                {
                    list.Add(item.Key + ": " + it.ErrorMessage);
                }
            }

            return list;
        }
    }
}
