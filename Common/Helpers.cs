using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class Helpers
    {
        /// <summary>
        /// Tells entity framework to pull specific navigation properties associated
        /// to a model when it executes the query.
        /// </summary>
        /// <typeparam name="TEntity">Model type</typeparam>
        /// <param name="source">Specifically the model <see cref="IQueryable{T}" or <see cref="DbSet{TEntity}"/>/></param>
        /// <param name="navigationProperties">List of related navigation properties</param>
        /// <returns>An <see cref="IQueryable{T}"/> with its child properties fetched.</returns>
        public static IQueryable<TEntity> IncludeWith<TEntity>(this IQueryable<TEntity> source, params string[] navigationProperties)
            where TEntity : class
        {

            for (int i = 0; i < navigationProperties.Length; i++)
            {
                string prop = navigationProperties[i];

                source = source.Include(prop);
            }

            return source;
        }
        /// <summary>
        /// Checks whether the project is in Debug configuration mode.
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                bool isdebug = true;
#if !DEBUG
                isdebug = false;
#endif
                return isdebug;
            }
        }
        /// <summary>
        /// Checks whether the project is in Release configuration mode.
        /// This configuration is mostly used in publishing or deployments.
        /// </summary>
        public static bool IsRelease
        {
            get
            {
                return !IsDebug;
            }
        }
        public static string DayHandler(this DateTime dateTime)
        {
            // check greater than or less than
            var span = dateTime.Subtract(DateTime.Now);

            int num = int.Parse(span.TotalDays.ToString().Split('.').First());

            if(num < -1)
            {
                return $"{span.Days*-1} days ago";
            }
            else if(num == -1)
            {
                return "yesterday";
            }
            else if(num == 0)
            {
                return "today";
            }
            else if(num == 1)
            {
                return "tomorrow";
            }
            else
            {
                return $"in {num} days";
            }
        }
    }
}
