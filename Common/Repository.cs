using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common
{
    public class Repository<T> : IRepository<T>,IDisposable where T : class
    {
        protected readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> List => All();
        private IQueryable<T> All()
        {
            //apply is deleted filter here
            Func<T, bool> isDeletedFuncer = (val) =>
             {
                 if ((bool)val.GetType().GetProperty("isDeleted").GetValue(val))
                     return false;
                 else
                     return true;
             };

            return _context.Set<T>()
                           .Where(isDeletedFuncer)
                           .AsQueryable();
        }
        public IEnumerable<T> ListWith(params string[] navigationProperties)
        {

            //apply is deleted filter here
            Func<T, bool> isDeletedFuncer = (val) =>
            {
                if ((bool)val.GetType().GetProperty("isDeleted").GetValue(val))
                    return false;
                else
                    return true;
            };

            return _context.Set<T>()
                           .IncludeWith(navigationProperties)
                           .Where(isDeletedFuncer);
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
            //set guid and timestamp universally
            typeof(T).GetProperty("uuid").SetValue(entity, Guid.NewGuid());

            typeof(T).GetProperty("timestamp").SetValue(entity, DateTime.Now);

            typeof(T).GetProperty("isDeleted").SetValue(entity, false);

            return _context.Set<T>().Add(entity).Entity;
        }



        #region Get Section
        public T Get(int id)
        {
            return _context.Set<T>().FirstOrDefault(x => (int)x.GetType().GetProperty("id").GetValue(x) == id);
        }
        public T Get(Guid uuid)
        {
            return _context.Set<T>()
                           .FirstOrDefault(x => Guid.Parse(x.GetType().GetProperty("uuid").GetValue(x).ToString()) == uuid);
        }
        public T GetWith(int id,params string[] navigationProperties)
        {
            Func<T, bool> IDFuncer = (val) =>
             {
                 if ((int)val.GetType().GetProperty("id").GetValue(val) == id)
                     return true;
                 else
                     return false;
             };
            //apply is deleted filter here
            Func<T, bool> isDeletedFuncer = (val) =>
            {
                if ((bool)val.GetType().GetProperty("isDeleted").GetValue(val))
                    return false;
                else
                    return true;
            };

            return _context.Set<T>()
                           .IncludeWith(navigationProperties)
                           .FirstOrDefault(IDFuncer);
        }
        public T GetWith(Guid uuid, params string[] navigationProperties)
        {
            Func<T, bool> IDFuncer = (val) =>
            {
                if ((Guid)val.GetType().GetProperty("uuid").GetValue(val) == uuid)
                    return true;
                else
                    return false;
            };
            //apply is deleted filter here
            Func<T, bool> isDeletedFuncer = (val) =>
            {
                if ((bool)val.GetType().GetProperty("isDeleted").GetValue(val))
                    return false;
                else
                    return true;
            };

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
            typeof(T).GetProperty("isDeleted").SetValue(entity, true);
            typeof(T).GetProperty("deletionDate").SetValue(entity, DateTime.Now);

            Update(entity);

            return true;
        }
        public bool Remove(int id)
        {
            var item = Get(id);

            item.GetType().GetProperty("isDeleted").SetValue(item, true);
            item.GetType().GetProperty("deletionDate").SetValue(item, DateTime.Now);

            Update(item);

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
