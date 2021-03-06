using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Data;
using SocialMedia.Infrastructure.Filters;
using SocialMedia.Infrastructure.Repositories;
using System;

namespace SocialMedia.Api
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

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

            services.AddControllers().AddNewtonsoftJson(options =>
            
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            ) .ConfigureApiBehaviorOptions(options => {
                //options.SuppressModelStateInvalidFilter = true; 
                });

            services.AddDbContext<SocialMediaContext>( options =>
                options.UseSqlServer(Configuration.GetConnectionString("SocialMediaDB"))
            );


            ConfigureRepositories(services);
            //ConfigureServicio(services);
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddFluentValidation(options =>
             {
               options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
             });
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        public void ConfigureRepositories(IServiceCollection services)
        {

            // Repositorios
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            // Servicios
            services.AddTransient<IPostService, PostService>();



        }

        public void ConfigureServicio(IServiceCollection services)
        {
            services.AddScoped<PostService>();

        }
    }
}
