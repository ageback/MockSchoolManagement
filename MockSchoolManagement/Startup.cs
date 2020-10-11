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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //�����ն��м������(app);

            ʵ���м����������(app, logger);

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
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
