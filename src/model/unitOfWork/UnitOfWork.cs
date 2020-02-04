using Microsoft.EntityFrameworkCore;
using mxcd.core.unitOfWork;
using mxcd.core.unitOfWork.enums;
using mxcd.dbContextExtended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace model.unitOfWork
{
    /// <summary>
    /// Unit of Work
    /// </summary>
    public class UnitOfWork : IEntityUnitOfWork
    {
        DbContextExtended Context { get; set; }
        List<Action> Actions = new List<Action>();
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(DbContextExtended context)
        {
            Context = context;
        }

        async Task IEntityUnitOfWork.Add<T>(T obj)
        {
            Context.Add(obj);
        }

        async Task IEntityUnitOfWork.Update<T>(T obj)
        {
            Context.Update(obj);
        }

        async Task IEntityUnitOfWork.Update<T, P>(Expression<Func<T, bool>> filter, P updateData)
        {
            Context.Update(updateData, filter);
        }

        async Task IEntityUnitOfWork.Remove<T>(T obj)
        {
            Context.Remove(obj);
        }

        async Task IEntityUnitOfWork.Remove<T>(Expression<Func<T, bool>> filter)
        {
            Context.Remove(filter);
        }

        IEnumerable<EntityState> ConvertTypes(IEnumerable<TypePending> types)
        {
            var result = new List<EntityState>();
            if (types.Contains(TypePending.add))
            {
                result.Add(EntityState.Added);
            }

            if (types.Contains(TypePending.update))
            {
                result.Add(EntityState.Modified);
            }

            if (types.Contains(TypePending.remove))
            {
                result.Add(EntityState.Deleted);
            }

            return result;
        }

        TypePending? ConvertTypes(EntityState type)
        {
            TypePending? result = null;

            switch (type)
            {
                case EntityState.Deleted:
                    result = TypePending.remove;
                    break;
                case EntityState.Modified:
                    result = TypePending.update;
                    break;
                case EntityState.Added:
                    result = TypePending.add;
                    break;
                default:
                    break;
            }

            return result;
        }

        class EntityPending<T>
        {
            public T Entity { get; set; }
            public EntityState State { get; set; }
        }

        IDictionary<TypePending, IEnumerable<TEntity>> GetPendingResult<TEntity>(IEnumerable<EntityPending<TEntity>> entries) where TEntity : class
        {
            return entries
                    .Select(x =>
                        new
                        {
                            x.Entity,
                            State = ConvertTypes(x.State)
                        }
                    )
                    .Where(x => x.State != null)
                    .GroupBy(x => x.State)
                    .ToDictionary(
                        x => x.Key.Value,
                        x => x.Select(y => y.Entity).ToList().AsEnumerable()
                    );
        }

        IDictionary<TypePending, IEnumerable<T>> IEntityUnitOfWork.GetPendingEntities<T>(params TypePending[] types)
        {
            var entries = Context.ChangeTracker.Entries<T>().ToList();
            if (types != null)
            {
                var convertTypes = ConvertTypes(types);
                entries = entries.Where(x => convertTypes.Contains(x.State)).ToList();
            }

            return GetPendingResult(entries.Select(x => new EntityPending<T>
            {
                Entity = x.Entity,
                State = x.State
            }));
        }

        IDictionary<TypePending, IEnumerable<object>> IEntityUnitOfWork.GetPendingEntities(params TypePending[] types)
        {
            var entries = Context.ChangeTracker.Entries()
                .Select(x =>
                new
                {
                    x.State,
                    x.Entity
                })
                .Where(x => x.Entity != null)
                .ToList();

            if (types != null)
            {
                var convertTypes = ConvertTypes(types);
                entries = entries.Where(x => convertTypes.Contains(x.State)).ToList();
            }

            return GetPendingResult(entries.Select(x => new EntityPending<object>
            {
                Entity = x.Entity,
                State = x.State
            }));
        }

        Task IUnitOfWork.SaveChanges()
        {
            return Task.Run(() =>
            {
                using (var scope = new TransactionScope())
                {
                    var tasks = new List<Task>();
                    tasks.Add(Context.SaveChangesAsync());
                    Actions.ForEach(x => tasks.Add(Task.Run(x)));

                    Task.WaitAll(tasks.ToArray());

                    scope.Complete();
                }
            });
        }

        Task IUnitOfWork.DiscardChanges()
        {
            Actions.Clear();
            return Context.DiscardChangesAsync();
        }

        Task IUnitOfWork.AddPendingAction(Action action)
        {
            return Task.Run(() => Actions.Add(action));
        }

        void IDisposable.Dispose()
        {
            Actions?.Clear();
            Actions = null;
            Context = null;
        }
    }
}
