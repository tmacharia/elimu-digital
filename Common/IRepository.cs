using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get A List of all objects in repository
        /// as an IEnumerable
        /// </summary>
        IEnumerable<T> List { get; }
        IEnumerable<T> ListWith(params string[] navigationProperties);
        /// <summary>
        /// Update the current entity in the specified repository by
        /// the generic class T
        /// </summary>
        /// <param name="entity">Object with new updated values</param>
        /// <returns>An updated object</returns>
        T Update(T entity);
        T[] UpdateMany(T[] entitie);

        /// <summary>
        /// Add a new item in the repository
        /// </summary>
        /// <param name="entity">Object entity to add in the repository</param>
        /// <returns>A newly created object after saving in the database</returns>
        T Create(T entity);
        /// <summary>
        /// Get an object by its primary key
        /// </summary>
        /// <param name="id">[Primary Key], Id</param>
        /// <returns>Object instance as found in the repository. Null if it does not exist.</returns>
        T Get(int id);
        T Get(Guid uuid);
        T Get(Func<T,bool> func);
        T GetWith(int id, params string[] navigationProperties);
        T GetWith(Guid uuid, params string[] navigationProperties);
        /// <summary>
        /// Get an object in a repository using a Func expression
        /// with a uuid for the object in question
        /// </summary>
        /// <param name="uuidExpression">Func Expression, specify a uuid</param>
        /// <returns>A found object</returns>
        T GetByGuid(Func<T, bool> uuidExpression);
        /// <summary>
        /// Gets an object from the repository based on the full object
        /// </summary>
        /// <param name="entity">The object to look for.</param>
        /// <returns>Found object. Null if not found</returns>
        T GetByObject(T entity);
        /// <summary>
        /// Gets an object from the repository based on a string key value
        /// 
        /// e.g email
        /// </summary>
        /// <param name="stringExpression">Func Expression containing a key as a string</param>
        /// <returns>Found object. Null if not found</returns>
        T GetByString(Func<T, bool> stringExpression);
        /// <summary>
        /// Gets a list of object that matches the predicate passed in here
        /// </summary>
        /// <param name="expression">Predicate as an expression func of any type</param>
        /// <returns>An IEnumerable of all objects matching search predicate</returns>
        //IEnumerable<T> Get(Func<T, bool> expression);

        bool Remove(T entity);
        bool Remove(int id);
        bool RemoveMany(int[] ids);
        bool RemoveMany(T[] entities);

        void Dispose();
    }
}
