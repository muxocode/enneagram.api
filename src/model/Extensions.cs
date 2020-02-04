using Microsoft.Extensions.DependencyInjection;
using model.repository;
using model.services;
using model.services.entity;
using model.unitOfWork;
using mxcd.core.repository;
using mxcd.core.rules;
using mxcd.core.rules.implementations;
using mxcd.core.services;
using mxcd.core.unitOfWork;

namespace model
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Add a IRepository
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddService<T>(this IServiceCollection services) where T:class
        {

            services.AddScoped<IService<T>, ServiceGeneric<T>>();
            //services.AddTransient<IRule<T>>(x => null);
            services.AddTransient<IRuleProcessor<T>, RuleProcessor<T>>();


            return services
                .AddScoped<IEntityRepository<T>, RepositoryGeneric<T>>()
                .AddScoped<IRepository<T>>(x => x.GetService<IEntityRepository<T>>());

        }
        /// <summary>
        /// Add IEntityUnitOfWork and IUnitOfWork
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services
               .AddScoped<IEntityUnitOfWork, UnitOfWork>()
               .AddScoped<IUnitOfWork>(x => x.GetService<IEntityUnitOfWork>());
        }
        /// <summary>
        /// Add a error handler
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddErrorHandler(this IServiceCollection services)
        {
            return services
               .AddTransient<IErrorHandler, ErrorHandler>();
        }
    }
}
