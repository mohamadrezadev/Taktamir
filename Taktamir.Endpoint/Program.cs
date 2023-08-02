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
using Taktamir.Endpoint.Hubs;
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
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseSqlite(builder.Configuration["sqliteconn"], b => b.MigrationsAssembly("Taktamir.infra.Data.sql"));
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //    options.EnableSensitiveDataLogging();
            //});
            builder.Services.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Taktamir.infra.Data.sql"));
                o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                o.EnableSensitiveDataLogging();
            });
            builder.Services.AddIdentity<User, Role>(option => 
            {
                option.SignIn.RequireConfirmedEmail = false;
                option.Password.RequireDigit = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.User.RequireUniqueEmail = false;
                option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //Singing Settings
                option.SignIn.RequireConfirmedEmail = false;
                option.SignIn.RequireConfirmedPhoneNumber = false;
                //Lockout Settings
                //option.Lockout.MaxFailedAccessAttempts = 10;
                //option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                option.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            #region Authenticationjwt
            builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:5001",
                    ValidAudience = "https://localhost:5001",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTRefreshTokenHIGHsecuredPasswordVVVp1OH7Xzyr"))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken)
                            && path.StartsWithSegments("/kitchen"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });




            #endregion
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
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
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Scheme = "Bearer",
                //    BearerFormat = "JWT",
                //    In = ParameterLocation.Header,
                //    Name = "Authorization",
                //    Description = "Bearer Authentication with JWT Token",
                //    Type = SecuritySchemeType.Http
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                //    {
                //        new OpenApiSecurityScheme {
                //            Reference = new OpenApiReference {
                //                Id = "Bearer",
                //                    Type = ReferenceType.SecurityScheme
                //            }
                //        },
                //        new List < string > ()
                //    }
                //});
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
            builder.Services.AddScoped<IVerifycodeRepository, VerifycodeRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        
            builder.Services.AddSingleton<IDictionary<string, UserConnection>>(new Dictionary<string, UserConnection>());

            builder.Services.AddScoped<ISmsService, SmsService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddTransient<ITokenService, TokenService>();

            //builder.Services.AddCors(options =>
            // {
            //     options.AddPolicy("CorsPolicy", builder =>
            //     {
            //         builder
            //             .AllowAnyMethod()
            //             .AllowAnyHeader()
            //             .WithOrigins("http://localhost:5173", "http://localhost:3006")
            //             .AllowCredentials();
            //     });
            //     //options.AddPolicy("CorsPolicy", builder =>
            //     //{
            //     //    builder.AllowAnyMethod()
            //     //           .AllowAnyHeader()
            //     //           .WithOrigins("http://localhost:3006")
            //     //           .AllowCredentials();
            //     //});
            // });
            builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:5173", "http://localhost:3006", " http://localhost:5173/");
            }));
            builder.Services.AddSignalR();

            var app = builder.Build();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root

            });
            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<ChatHub>("/chats");
            //app.UseSentryTracing();
            app.Run();
        }
    }
}