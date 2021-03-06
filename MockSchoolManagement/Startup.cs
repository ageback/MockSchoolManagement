using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.OpenApi.Models;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Students;
using MockSchoolManagement.CustomerMiddlewares;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Infrastructure.Data;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.Security;
using MockSchoolManagement.Security.CustomTokenProvider;
using NetCore.AutoRegisterDi;

namespace MockSchoolManagement
{
    public class Startup
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this._configuration = configuration;
            this._env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MockSchoolManagement PI",
                    Version = "v1",
                    Description = "为 MockSchoolManagement 系统，添加一个简单的 ASP.NET Core Web API 示例。",
                    TermsOfService = new Uri("http://192.168.0.241:5000"),
                    Contact = new OpenApiContact
                    {
                        Name = "LMH",
                        Email = "lmh@wesoft.net.cn",
                        Url = new Uri("http://192.168.0.241:5000")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache License 2.0",
                        Url = new Uri("http://github.com")
                    }
                });
                if (_env.IsDevelopment())
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                } 
            });
            // 设置所有令牌有效期为5小时
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));
            // 仅更改电子邮箱验证令牌类型的有效期为3天
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));
            services.AddAuthentication().AddMicrosoftAccount(msOptions =>
            {
                msOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
                msOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            }).AddGitHub(options=> { 
                options.ClientId=_configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret=_configuration["Authentication:GitHub:ClientSecret"]; 
            });
            services.AddHttpContextAccessor();
            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = new PathString("/Admin/AccessDenied"));
            // 策略结合声明授权
            services.AddAuthorization(options => {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();

            // 自动注入服务到依赖注入容器
            services.RegisterAssemblyPublicNonGenericClasses().Where(c => { return c.Name.EndsWith("Service") || c.Name.EndsWith("Repository"); }).AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
            // 使用 sqlserver 数据库，通过IConfiguration访问去获取，自定义名称的MockStudentDBConnection作为连接字符串
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));
            // 禁用一些密码策略
            services.Configure<IdentityOptions>(options => { 
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = true;

                //通过自定义的CustomEmailConfirmation名称来覆盖旧有的token名称，
                //是它与AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation")关联在一起
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                // 密码错误5次锁定账号，15分钟
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");
            services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));
            var builder = services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            if (_env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // 数据初始化
            app.UseDataInitializer();
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
                // 显示用户友好的错误页面
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MockSchoolManagement API V1");
            });
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(ep =>
            {
                ep.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// 自定义授权策略委托方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            var contextUser = context.User;
            return contextUser.IsInRole("Admin") &&
                contextUser.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                contextUser.IsInRole("Super Admin");
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
