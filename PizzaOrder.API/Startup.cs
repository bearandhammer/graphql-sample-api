using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PizzaOrder.API.Extensions;
using PizzaOrder.Data;

namespace PizzaOrder.API
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
            services.AddControllers();
            services.AddDbContext<PizzaDBContext>(
                optionsAction: options => options.UseSqlServer(Configuration["ConnectionStrings:PizzaOrderDB"]),
                contextLifetime: ServiceLifetime.Singleton);

            services.AddCustomServices();
            services.AddCustomGraphQLServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PizzaDBContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Seed sample data...
            dbContext.EnsureDataSeeding();

            app.UseWebSockets();
            app.UseGraphQLPlayground();
        }
    }
}
