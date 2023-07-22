using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Core.Domain._08.Verifycodes;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.Endpoint.Profiles;
using Taktamir.framework;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._02.Jobs;
using Taktamir.infra.Data.sql._03.Users;
using Taktamir.infra.Data.sql._04.Customers;
using Taktamir.infra.Data.sql._05.Messages;
using Taktamir.infra.Data.sql._06.Wallets;
using Taktamir.infra.Data.sql._07.Suppliess;
using Taktamir.infra.Data.sql._08.Verifycodes;
using Taktamir.Services.JwtServices;
using Taktamir.Services.SmsServices;

namespace Taktamir.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureLogging(option =>
            {
                option.ClearProviders();
                option.AddConsole();
            }).UseNLog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Taktamir API",
                    Version = "v1",
                    //Description = "An API to perform Jobs operations",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "mohamadrezakiani",
                        Email = "mohamadrezakiani9@yahoo.com",
                       // Url = new Uri("https://twitter.com/jwalkner"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "taktamir API LICX",
                        // Url = new Uri("https://example.com/license"),
                    }
                });
        
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme, e.g. \"Bearer {token} \"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration["sqliteconn"], b => b.MigrationsAssembly("Taktamir.infra.Data.sql"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging();
            });

            #region Authenticationjwt
            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });
            #endregion
            builder.Services.AddIdentity<User, Role>(o =>{})
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
           
            builder.Services.Configure<KavenegarConfig>(builder.Configuration.GetSection("Kavenegarinfo"));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IWalletRepository, WalletsRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ISuppliesRepository, SuppliesRepository>();
            builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();
            builder.Services.AddScoped<IOrderRepository, OrdersRepository>();
            builder.Services.AddScoped<IVerifycodeRepository, VerifycodeRepository>();
            builder.Services.AddScoped<ISmsService, SmsService>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            var app = builder.Build();

            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root

            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            //app.UseSentryTracing();

            app.Run();
        }
    }
}