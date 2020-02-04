using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mxcd.dbContextExtended;

namespace data.extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection AddEnneagramContext(this IServiceCollection services, string connectionString, ILoggerFactory loggerFactory=null)
        {
            services.AddDbContext<data.EnneagramContext>(opt =>
            {              
                opt.UseSqlServer(connectionString);
                if (loggerFactory != null)
                    opt.UseLoggerFactory(loggerFactory);
            });

            services.AddScoped<DbContext>(x => x.GetService<EnneagramContext>());
            services.AddScoped<DbContextExtended>(x => x.GetService<EnneagramContext>());

            return services;
        }
    }
}
