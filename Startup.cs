using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using webApi2.Context;
using webApi2.Service;
using webApi2.Service.InterfaceService;

namespace webApi2
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
            

            services.AddScoped<IRegisterService,RegisterService>();
            services.AddScoped<ILoginService,LoginService>();
            services.AddScoped<Itimbrar,Timbrar>();


            services.AddHttpClient();

            //Configuring the cors
            services.AddCors(cors=>{
                cors.AddPolicy("CorsOrigins",c=>c.WithOrigins("*"));
            });


            string secretKey="ihspidhasdASD2123123ASJDA123123SDKOQW@$%#ksdoh$%=)88619821$$%%132";

            var key = Encoding.ASCII.GetBytes(secretKey);


            services.AddAuthentication(x=>{
                x.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(jwt=>
            {
                jwt.RequireHttpsMetadata=false;
                jwt.SaveToken=false;

                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

            });

            services.AddAuthorization(policy=>{
                policy.AddPolicy("SuperUser",superUser=>{
                    superUser.RequireClaim("Role","true");
                });
            });



            /*services.AddDbContext<AppDbContext>(options=>options.UseMySql("server=localhost;database=library;user=user;password=password");*/

            services.AddDbContext<AppDbContext>(options=>options.UseMySql(Configuration.GetConnectionString("Default"),new MySqlServerVersion(new Version(8, 0, 11))));
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "webApi2", Version = "v1" });
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "webApi2 v1"));
            }
            
            app.UseCors("CorsOrigins");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
