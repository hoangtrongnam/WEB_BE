// using Oracle.ManagedDataAccess.Client;
// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WEB_BANHANG_BE.Persistence;
using WEB_BANHANG_BE.Services.Logger;
using WEB_BANHANG_BE.Extensions;
using WebApi.Services.ProductService;

namespace WEB_BANHANG_BE
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            // loàng ngoằng bí hiểm
            //LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            // 2 thành phần chính của Swashbuckle:
            // 1. Swashbuckle.SwaggerGen: cung cấp các chức năng để tạo ra tài liệu JSON Swagger mô tả các đối tượng, phương thức, kiểu dữ liệu trả về, tham số, ...
            // 2. Swashbuckle.SwaggerUI: tích hợp phiên bản Swagger UI, cái mà sử dụng chỉnh sửa mô tả, chức năng của Web API.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RWA API", Version = "v1" });
                c.SwaggerDoc("v1.1", new OpenApiInfo { Title = "RWA API", Version = "v1.1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
                c.OperationFilter<RemoveVersionParameterFilter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
            // Other code omitted
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                //Các  AssumeDefaultVersionWhenUnspecified cờ được sử dụng để thiết lập các phiên bản mặc định khi khách hàng đã không quy định bất kỳ phiên bản. Không có cờ này,  UnsupportedApiVersion ngoại lệ sẽ xảy ra khi phiên bản không được chỉ định bởi máy khách.
                o.DefaultApiVersion = new ApiVersion(1, 0);
                // Các  DefaultApiVersion cờ được sử dụng để thiết lập các số phiên bản mặc định
            });
            // Lowercase urls (viet thuong duong dan)
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson(options => {
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new LowercaseContractResolver();
            });

            AuthConfigurer.Configure(services, _configuration);
            //This config for allow localhost connect. For development stage only, remove when deploy production (mở kết nối với font end khi phát triển, xóa khi live)
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            this._configuration["App:CorsOrigins"]// đường dẫn kết nối appsitting http://localhost:4200
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                //.Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );
            // lấy chuổi kết nối data
            var connectionDict = new Dictionary<DatabaseConnectionName, string>
            {
                { DatabaseConnectionName.Connection1, this._configuration.GetConnectionString("DefaultConnection")??""},
                { DatabaseConnectionName.Connection2, this._configuration.GetConnectionString("DefaultConnection")??""}
            };

            services.AddDataProtection();
            // Inject this dict 
            services.AddSingleton<IDictionary<DatabaseConnectionName, string>>(connectionDict);
            // LoggerManager định nghĩa lỗi
            services.AddSingleton<ILoggerManager, LoggerManager>();

            // Inject the factory
            services.AddTransient<IDbConnectionFactory, DapperDbConnectionFactory>();

            // Register your regular repositories
            services.AddScoped<ModelValidationAttribute>();
            services.AddScoped<IProductService, ProductService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.SwaggerEndpoint("/swagger/v1.1/swagger.json", "My API V1.1");
                });
            }
            // định nghĩa ExceptionHandler
            app.ConfigureExceptionHandler(logger);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(_defaultCorsPolicyName);// Enable CORS! CORS là mở kết nối font end khi phát triển

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the Localization middleware
            var cultureInfo = new CultureInfo("en-US");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(cultureInfo),
                SupportedCultures = new List<CultureInfo>
                {
                    cultureInfo,
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    cultureInfo,
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
