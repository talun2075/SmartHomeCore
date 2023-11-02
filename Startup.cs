using Bootstrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SmartHome
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
            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.IncludeFields = true);
            services.AddConfig(Configuration);//Bootstrapper
            services.AddServices(Configuration);//Bootstrapper
            services.AddCors(policyBuilder =>policyBuilder.AddDefaultPolicy(policy =>policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();//=> f�r index.html Dateien; muss vor UseStaticFiles stehen
            app.UseStaticFiles();
            app.UseCors();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
          
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
