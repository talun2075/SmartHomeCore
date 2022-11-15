using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Classes;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();//=> für index.html Dateien; muss vor UseStaticFiles stehen
            app.UseStaticFiles();
            app.UseRouting();

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode != 200 || context.Request.Host.Value == "apollo.nanoleaf.me")
            //    {
            //        var p = context.Request.Host.Value + context.Request.Path.Value;
            //        //apollo.nanoleaf.me
            //        SmartHomeConstants.log.InfoLog("Startup", p);
            //        await next();
            //    }
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
