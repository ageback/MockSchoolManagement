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
            // ��������������Ч��Ϊ5Сʱ
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));
            // �����ĵ���������֤�������͵���Ч��Ϊ3��
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
            // ���Խ��������Ȩ
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

            // �Զ�ע���������ע������
            services.RegisterAssemblyPublicNonGenericClasses().Where(c => { return c.Name.EndsWith("Service") || c.Name.EndsWith("Repository"); }).AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
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
                options.SignIn.RequireConfirmedEmail = true;

                //ͨ���Զ����CustomEmailConfirmation���������Ǿ��е�token���ƣ�
                //������AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation")������һ��
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                // �������5�������˺ţ�15����
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
            // ���ݳ�ʼ��
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

        /// <summary>
        /// �Զ�����Ȩ����ί�з���
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
