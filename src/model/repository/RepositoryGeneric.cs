using entities;
using Microsoft.EntityFrameworkCore;
using mxcd.core.repository;
using mxcd.core.unitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace model.repository
{
    //TODO: Poner un buffer de GET, peor antes ver con un profiler
    //TODO: Comprobar si hacemos un update, lo devuelve modificado
    internal class RepositoryGeneric<T> : IEntityRepository<T> where T : class
    {
        protected DbContext Context { get; }
        protected IEntityUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="unitOfWork"></param>
        public RepositoryGeneric(DbContext context, IEntityUnitOfWork unitOfWork)
        {
            Context = context;
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds an entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Task Add(T item)
        {
            return UnitOfWork.Add(item);
        }

        /// <summary>
        /// Removes with expression, it will be executed on db transaction
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Task Remove(Expression<Func<T, bool>> expression)
        {
            return UnitOfWork.Remove(expression);
        }
        /// <summary>
        /// Removes by key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Task Remove<TKey>(TKey key)
        {
            //TODO: Mirar esto
            //return UnitOfWork.Remove<T>(x => x.Id.Equals(key));
            return UnitOfWork.Remove<T>();

        }
        /// <summary>
        /// Removes a set of items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual Task Remove(params T[] items)
        {
            Task result;

            if(items!=null)
            {
                //TODO: Mirar esto
                //result = Task.WhenAll(items.Select(x => Remove(x.GetKey())));

                result = null;
            }
            else
            {
                result = UnitOfWork.Remove<T>();
            }

            return result;
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            
        }
        /// <summary>
        /// Get a set of entities
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Task<T> Find<TKey>(TKey key)
        {
            return Context.FindAsync<T>(key).AsTask();
        }
        /// <summary>
        /// Get a set of entities
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<T>> Get(Expression<Func<T, bool>> expression = null)
        {
            var set = Context.Set<T>();
            IEnumerable<T> query = null;
            if(expression!=null)
            {
                query = set.Where(expression.Compile());
            }
            return Task.FromResult(query??set.AsEnumerable());
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual Task Update(T obj)
        {
            return UnitOfWork.Update(obj);
        }
        /// <summary>
        /// Updates by expression
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="expression"></param>
        /// <param name="objToUpdate"></param>
        /// <returns></returns>
        public virtual Task Update<TUpdate>(Expression<Func<T, bool>> expression, TUpdate objToUpdate) where TUpdate:class
        {
            return UnitOfWork.Update(expression, objToUpdate);
        }        
    }
}
