using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.DataRepositories;

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
            services.AddControllersWithViews(a => a.EnableEndpointRouting = false).AddXmlSerializerFormatters();
            services.AddSingleton<IStudentRepository, MockStudentRepository>();
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

            //app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});


            //测试终端中间件特性(app);

            //实践中间件工作流程(app, logger);



            //app.Run(async (context) => {
            //    throw new Exception("主动抛异常");
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseRouting();
            app.UseEndpoints(ep =>
            {
                ep.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }

        private static void 实践中间件工作流程(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                context.Response.ContentType = "text/plain;charset=utf-8";
                logger.LogInformation("MW1:传入请求");
                await next();
                logger.LogInformation("MW1:传出请求");

            });

            app.Use(async (context, next) =>
            {
                logger.LogInformation("MW2:传入请求");
                await next();
                logger.LogInformation("MW2:传出请求");

            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("MW3: 处理请求并生成响应");
                logger.LogInformation("MW3:处理请求并生成响应");
            });
        }

        private static void 测试终端中间件特性(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                // 防止乱码
                context.Response.ContentType = "text/plain;charset=utf-8";
                await context.Response.WriteAsync("从第一个中间件中打印Hello World.");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("从第二个中间件中打印Hello World.");
            });
        }
    }
}
