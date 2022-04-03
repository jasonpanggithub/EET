using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EET.Service;

namespace EET
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Install-Package Microsoft.AspNetCore.Mvc.NewtonsoftJson -Version 3.1.2
            // to avoid the problem for converting integer to string for JSON
            services.AddControllers().AddNewtonsoftJson(); 
            services.AddSingleton<IMetaDataService>(new MetaDataService(Configuration));
            services.AddSingleton<IMovieStatistic>(new MovieStatisticServicecs(Configuration));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
