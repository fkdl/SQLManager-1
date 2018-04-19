using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SQLManager
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name: "default",
            //         //template: "{controller=Home}/{action=Index}/{Name?}");
            //         template: "{controller}",
            //         defaults: new { controller = "Home", action = "Index"});
            // });

            app.UseMvc(routes =>
            {
                routes.MapRoute("table", "Table/{Name?}",
                        defaults: new { controller = "Table", action = "Index" });
                routes.MapRoute("database", "Database/{Name?}",
                        defaults: new { controller = "Database", action = "Index" });
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{Name?}");
            });
        }
    }
}
