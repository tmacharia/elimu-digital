using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common
{
    public class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        protected readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> List => All();
        private IQueryable<T> All()
        {
            return _context.Set<T>()
                           .AsQueryable();
        }
        public IEnumerable<T> ListWith(params string[] navigationProperties)
        {
            return _context.Set<T>()
                           .IncludeWith(navigationProperties);
        }
        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            return entity;
        }
        public T[] UpdateMany(T[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                _context.Entry(entities[i]).State = EntityState.Modified;
            }

            return entities;
        }
        public T Create(T entity)
        {
            return _context.Set<T>().Add(entity).Entity;
        }



        #region Get Section
        public T Get(int Id)
        {
            return _context.Set<T>()
                           .FirstOrDefault(x => (int)x.GetType()
                           .GetProperty("Id")
                           .GetValue(x) == Id);
        }
        public T Get(Guid uuid)
        {
            return _context.Set<T>()
                           .FirstOrDefault(x => Guid.Parse(x.GetType()
                           .GetProperty("uuid")
                           .GetValue(x)
                           .ToString()) == uuid);
        }
        public T GetWith(int Id,params string[] navigationProperties)
        {
            bool IDFuncer(T val)
            {
                if ((int)val.GetType().GetProperty("Id").GetValue(val) == Id)
                    return true;
                else
                    return false;
            }

            return _context.Set<T>()
                           .IncludeWith(navigationProperties)
                           .FirstOrDefault(IDFuncer);
        }
        public T GetWith(Guid uuid, params string[] navigationProperties)
        {
            bool IDFuncer(T val)
            {
                if ((Guid)val.GetType().GetProperty("uuid").GetValue(val) == uuid)
                    return true;
                else
                    return false;
            }

            return _context.Set<T>()
                           .IncludeWith(navigationProperties)
                           .FirstOrDefault(IDFuncer);
        }

        public T GetByObject(T entity)
        {
            return _context.Set<T>().Find(entity);
        }
        public T GetByString(Func<T, bool> stringFunc)
        {
            return _context.Set<T>().FirstOrDefault(stringFunc);
        }
        public T GetByGuid(Func<T, bool> uuidFunc)
        {
            return _context.Set<T>().First(uuidFunc);
        }
        public IEnumerable<T> Get(Func<T, bool> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        #endregion



        public bool Remove(T entity)
        {
            _context.Set<T>().Remove(entity);

            return true;
        }
        public bool Remove(int Id)
        {
            var item = Get(Id);

            Remove(item);

            return true;
        }
        public bool RemoveMany(int[] ids)
        {
            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    Remove(ids[i]);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveMany(T[] entities)
        {
            try
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Remove(entities[i]);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
