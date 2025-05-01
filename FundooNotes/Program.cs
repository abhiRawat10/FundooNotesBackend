using BuisnessLayer.InterFaces;
using BuisnessLayer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RabbitMqConsumer;
using RepositoryLayer.Middlewares;
using NLog.Web;
using System.Reflection.Metadata;
using StackExchange.Redis;
using BuisnessLayer.Interfaces;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Replace default logger with NLog
                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(LogLevel.Trace);
                builder.Host.UseNLog();

                builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CONN")));
                // Add services to the container.
                builder.Services.AddScoped<IUserBL, UserImplBL>();
                builder.Services.AddScoped<IUserRL, UserImplRL>();
                builder.Services.AddScoped<AuthService>();
                builder.Services.AddScoped<INotesRL, NotesImplRL>();
                builder.Services.AddScoped<INotesBL, NotesImplBL>();
                builder.Services.AddScoped<INoteCollaboratorRL, NoteCollaboratorImplRL>();
                builder.Services.AddScoped<INoteCollaboratorBL, NoteCollaboratorImplBL>();
                builder.Services.AddScoped<ILabelRL, LabelImplRL>();
                builder.Services.AddScoped<ILabelBL, LabelImplBL>();

                builder.Services.AddHostedService<RabbitMqConsumerService>();


                //for redis

                builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var configuration = builder.Configuration.GetConnectionString("Redis");
                    return ConnectionMultiplexer.Connect(configuration);
                });

                builder.Services.AddSingleton(sp =>
                {
                    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                    return multiplexer.GetDatabase(); // inject IDatabase directly
                });

                //for global function

                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    Exception ex = (Exception)args.ExceptionObject;
                    Console.WriteLine("Unhandled Exception: " + ex.Message);
                };

                TaskScheduler.UnobservedTaskException += (sender, args) =>
                {
                    Console.WriteLine("Unobserved Task Exception: " + args.Exception.Message);
                    args.SetObserved();
                };

               //SWAGGER
                builder.Services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Enter Token only",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { } // No specific scopes required
                        }
                    });
                });

                //for automatic middleware to handle jwt token
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                        };
                    });

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();


                // Load YARP configuration from appsettings.json
                builder.Services.AddReverseProxy()
                    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

                var app = builder.Build();

                app.MapReverseProxy();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                //app.UseMiddleware<GlobalExceptionMiddleware>();
                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();


                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of an exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
