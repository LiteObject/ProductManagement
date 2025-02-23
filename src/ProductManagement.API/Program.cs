
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ProductManagement.App.Profiles;
using ProductManagement.App.Services;
using ProductManagement.Core.Interfaces;
using ProductManagement.Infra.Contexts;
using ProductManagement.Infra.Repositories;
using Scalar.AspNetCore;

namespace ProductManagement.API
{
    /// <summary>
    /// Program class for the API.
    /// </summary>
    public sealed partial class Program
    {
        /// <summary>
        /// Main method for the API.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add explicit configuration (optional if you need specific options)
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(); // Document name is v1
            builder.Services.AddOpenApi("internal"); // Document name is internal

            // Register the ProductContext with the dependency injection container
            builder.Services.AddDbContext<ProductContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register IProductRepo with its implementation
            builder.Services.AddScoped<IProductService, ProductService>(); // Register IProductService with its implementation

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi("/openapi/v1/openapi.json");
                
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
                    options.RoutePrefix = "swagger"; // Set the Swagger UI at the root URL
                    options.DocumentTitle = "Product Management API Documentation"; // Set the document title
                    options.DisplayRequestDuration(); // Display request duration
                });

                app.MapScalarApiReference();

                //await using var serviceScope = app.Services.CreateAsyncScope();
                //await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ProductContext>();
                //await dbContext.Database.EnsureCreatedAsync();

                app.UseDeveloperExceptionPage();
            }

            // app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseExceptionHandler();
            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Map health check endpoints
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/db", new HealthCheckOptions
            {
                Predicate = check => check.Name.Contains("ProductContext")
            });

            app.Run();
        }
    }
}
