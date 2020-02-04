using data.extensions;
using entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OData.Edm;
using model;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string conString = ConfigurationExtensions
                               .GetConnectionString(this.Configuration, "enneagram");

            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddDebug()
                       .AddConsole();
            }
);

            services.AddEnneagramContext(conString, loggerFactory);

            services.AddService<Enneatype>();
            services.AddUnitOfWork();
            services.AddErrorHandler();

            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddOData();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Filter().Count().Expand().OrderBy().MaxTop(100);
                routeBuilder.MapODataServiceRoute("api", "api", GetEdmModel());
            });

            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });*/

            IEdmModel GetEdmModel()
            {
                var odataBuilder = new ODataConventionModelBuilder();

                odataBuilder.EntitySet<Enneatype>(typeof(Enneatype).Name);

                return odataBuilder.GetEdmModel();
            }
        }
    }
}
