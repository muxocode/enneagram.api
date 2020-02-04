using mxcd.core.repository;
using mxcd.core.rules;
using mxcd.core.services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace model.services.entity
{
    internal class ServiceGeneric<T> : IService<T> where T : class
    { 
        readonly IEntityRepository<T> Repository;
        readonly IRuleProcessor<T> RuleProcesor;
        public ServiceGeneric(IEntityRepository<T> repository, IRuleProcessor<T> ruleProcesor)
        {
            Repository = repository;
            RuleProcesor = ruleProcesor;
        }



        public Task<T> Find<TKey>(TKey key)
        {
            return this.Repository.Find(key);
        }

        public Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter = null)
        {
            return this.Repository.Get(expression: filter);
        }

        public async Task<T> Insert(T obj)
        {
            await RuleProcesor.CheckRules(obj);
            await Repository.Add(obj);
            return obj;
        }

        public Task Remove<TKey>(TKey key)
        {
            return Repository.Remove(key);
        }

        public Task Remove(T obj)
        {
            return Repository.Remove(obj);
        }

        public async Task Update(T obj)
        {
            await RuleProcesor.CheckRules(obj);
            await Repository.Update(obj);
        }
    }
}
