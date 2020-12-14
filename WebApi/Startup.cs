using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Token.Provider;
using WebApi.Extentions;

namespace WebApi
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
            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors(options =>
            {
                options.AddPolicy("corsAllowAllPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            string connectionString = Configuration.GetConnectionString("Default");
            string infraestructureAssemblyNamespace = Configuration.GetSection("AppConfig:InfraestructureAssemblyNamespace").Value;
            string logicAssemblyNamespace = Configuration.GetSection("AppConfig:LogicAssemblyNamespace").Value;
            var infraestructureAssembly = Assembly.Load(new AssemblyName(infraestructureAssemblyNamespace));
            var logicAssembly = Assembly.Load(new AssemblyName(logicAssemblyNamespace));

            RegisterScopedServices(services, infraestructureAssembly, "Repository");
            RegisterScopedServices(services, logicAssembly, "Logic");
            services.AddNHibernate(connectionString);
            services.AddAutenticationToken(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("corsAllowAllPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }

        void RegisterScopedServices(IServiceCollection services, Assembly assembly, string subfix)
        {
            foreach (var type in assembly.ExportedTypes.Where(e => e.Name.EndsWith(subfix)))
            {
                var interfaces = type.GetInterfaces().Where(i => i.Name.EndsWith(subfix)).ToList();
                foreach (var f in interfaces)
                {
                    services.AddScoped(f, type);
                }
            }
        }
    }
}
