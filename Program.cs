
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PerformanceSurvey.Context;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Repository;
using PerformanceSurvey.Services;
using System.Text;
using System.Text.Json.Serialization;
using PerformanceSurvey.Configuration;
using PerformanceSurvey.Utilities;
using PerformanceSurvey.Models.RequestDTOs;
using PerformanceSurvey.Middlewares;
namespace PerformanceSurvey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Register ApplicationDbContext


            //builder.Services.AddDbContext<ApplicationDbContext>(Options =>

            //{

            //    Options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

            //});

            builder.Services.AddDbContext<ApplicationDbContext>(Options =>

            {

                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            });

            // Configure JSON serialization options
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    // Optional: Configure other settings if needed
                    // options.JsonSerializerOptions.MaxDepth = 64;
                });
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policy =>
                    {
                        policy.AllowAnyOrigin()  // Allow all origins
                              .AllowAnyHeader()  // Allow any headers
                              .AllowAnyMethod(); // Allow any HTTP methods
                    });
            });


            // Register repositories
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IAssignmentQuestionRepository, AssignmentQuestionRepository>();
            
            builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
            builder.Services.AddScoped<IAuthLoginRepository, AuthLoginRepository>();



            builder.Services.AddScoped<IAuthLoginService, AuthLoginService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAssignmentQuestionService, AssignmentQuestionService>();
            builder.Services.AddScoped<IResponseService, ResponseService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<JwtTokenUtil>();
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettings);


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),

            };


        });



            // Configure JWT Authentication
            //var jwtSettings = builder.Configuration.GetSection("Jwt");
            //builder.Services.Configure<JwtSettings>(jwtSettings);

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    var key = builder.Configuration["Jwt:Key"];
            //    var issuer = builder.Configuration["Jwt:Issuer"];
            //    var audience = builder.Configuration["Jwt:Audience"];

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = issuer,
            //        ValidAudience = audience,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            //    };
            //});

            // Add authorization services
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Define example for DepartmentQuestionDto
                c.MapType<MultipleChoiceQuestionRequest>(() => new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["QuestionText"] = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("Rate posting accuracy")
                        },
                        ["DepartmentId"] = new OpenApiSchema
                        {
                            Type = "integer",
                            Example = new OpenApiInteger(3)
                        },
                        ["Options"] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["Text"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Example = new OpenApiString("Very Poor")
                                    },
                                    ["Score"] = new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Example = new OpenApiInteger(20)
                                    }
                                }
                            },
                            Example = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["Text"] = new OpenApiString("Very Poor"),
                        ["Score"] = new OpenApiInteger(20)
                    },
                    new OpenApiObject
                    {
                        ["Text"] = new OpenApiString("Poor"),
                        ["Score"] = new OpenApiInteger(40)
                    },
                    new OpenApiObject
                    {
                        ["Text"] = new OpenApiString("Good"),
                        ["Score"] = new OpenApiInteger(60)
                    },
                    new OpenApiObject
                    {
                        ["Text"] = new OpenApiString("Very Good"),
                        ["Score"] = new OpenApiInteger(80)
                    },
                    new OpenApiObject
                    {
                        ["Text"] = new OpenApiString("Excellent"),
                        ["Score"] = new OpenApiInteger(100)
                    }
                }
                        }
                    }
                });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer 12345abcdef\""
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
              new string[] {}
        }
    });

            });
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{

            //    app.UseDeveloperExceptionPage();
            //}
            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/SurveyApi/swagger/v1/swagger.json", "PerformanceSurvey API V1"); // Use full path
                //c.RoutePrefix = string.Empty;
                //on production
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PerformanceSurvey API V1");
            });

            app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    c.RoutePrefix = string.Empty; // This makes the Swagger UI load at the root URL
            //});
            app.UseCors("AllowAllOrigins");

            app.UseMiddleware<TokenRevocationMiddleware>();
            //app.UseHttpsRedirection();
            app.UseRouting(); // Ensure routing is used
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            //app.Urls.Add("http://*:8080");
            app.Run();
        }
    }
}
