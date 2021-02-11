using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.CustomerMiddlewares;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;

namespace MockSchoolManagement
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ʹ�� sqlserver ���ݿ⣬ͨ��IConfiguration����ȥ��ȡ���Զ������Ƶ�MockStudentDBConnection��Ϊ�����ַ���
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));
            // ����һЩ�������
            services.Configure<IdentityOptions>(options => { 
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            });
            services.AddIdentity<IdentityUser, IdentityRole>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();
            //services.AddControllersWithViews(a => a.EnableEndpointRouting = false).AddXmlSerializerFormatters();
            services.AddScoped<IStudentRepository, SQLStudentRepository>();
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            //services.AddSingleton<IStudentRepository, MockStudentRepository>();
            //services.AddScoped<IStudentRepository, MockStudentRepository>();
            //services.AddTransient<IStudentRepository, MockStudentRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                DeveloperExceptionPageOptions options = new DeveloperExceptionPageOptions
                {
                    SourceCodeLineCount = 3
                };

                app.UseDeveloperExceptionPage(options);
            }
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            {
                // ��ʾ�û��ѺõĴ���ҳ��
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            

            //app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(ep =>
            {
                ep.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ʵ���м����������(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                context.Response.ContentType = "text/plain;charset=utf-8";
                logger.LogInformation("MW1:��������");
                await next();
                logger.LogInformation("MW1:��������");

            });

            app.Use(async (context, next) =>
            {
                logger.LogInformation("MW2:��������");
                await next();
                logger.LogInformation("MW2:��������");

            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("MW3: ��������������Ӧ");
                logger.LogInformation("MW3:��������������Ӧ");
            });
        }

        private static void �����ն��м������(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                // ��ֹ����
                context.Response.ContentType = "text/plain;charset=utf-8";
                await context.Response.WriteAsync("�ӵ�һ���м���д�ӡHello World.");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("�ӵڶ����м���д�ӡHello World.");
            });
        }
    }
}
