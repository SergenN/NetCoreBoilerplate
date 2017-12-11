using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetCoreBoilerplate.Database;
using NetCoreBoilerplate.Models.Database;
using NetCoreBoilerplate.Models.Settings;
using NetCoreBoilerplate.Services.Auth;
using NetCoreBoilerplate.Services.Messaging;

namespace NetCoreBoilerplate
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
            // ===== Add framework services =====
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // ===== Appsettings to Models.Settings =====
            var jwtAppSetting = Configuration.GetSection(nameof(JwtSettings));
            services.Configure<MailSettings>(Configuration.GetSection(nameof(MailSettings)));
            services.Configure<JwtSettings>(jwtAppSetting);
            
            // ===== Add injectables =====
            services.AddSingleton<IAuthService, AuthService>();
            services.AddTransient<IEmailSender, MessageService>();
            services.AddTransient<ISmsSender, MessageService>();
            
            // ===== Add Jwt Authentication =====
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtAppSetting[nameof(JwtSettings.Issuer)],
                        ValidAudience = jwtAppSetting[nameof(JwtSettings.Audience)],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSetting[nameof(JwtSettings.SecretKey)])),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            
            
            // ===== Identity Password settings =====
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
            
            // ===== Final add MVC =====
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseAuthentication();
            app.UseMvc();
            DbSeed.Initialize(dbContext);
            dbContext.Database.EnsureCreated();
        }
    }
}