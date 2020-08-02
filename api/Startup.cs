using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.Configure<OktaSettings>(Configuration.GetSection("Okta"));
            var okta = Configuration.GetSection("Okta").Get<OktaSettings>();
            Console.WriteLine(okta.ToString());
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // both values from from API->Authorization Servers
                // if don't match exactly, get 401 returned to client
                options.Authority = okta.Authority;
                options.Audience = okta.Audience;
                options.RequireHttpsMetadata = false;
            });
            services.AddControllers();
            services.AddAuthorization( options => {
                options.AddPolicy("AddScope", policy =>
                    policy.Requirements.Add(new ScopeRequirement("add_item")));
                options.AddPolicy("GetScope", policy =>
                    policy.Requirements.Add(new ScopeRequirement("get_item")));
            } );
            services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
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

            app.UseAuthentication(); // added for Okta

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
