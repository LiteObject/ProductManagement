
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ProductManagement.App.Profiles;
using ProductManagement.App.Services;
using ProductManagement.Core.Interfaces;
using ProductManagement.Infra.Contexts;
using ProductManagement.Infra.Repositories;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

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
            builder.Services.AddControllers(options => {

                // Will return "406 Not Acceptable" if formatter doesn't match "Accept" header.
                options.ReturnHttpNotAcceptable = true;
            });

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = (context) =>
                {                   
                    context.ProblemDetails.Extensions.Add("Host", Environment.MachineName);

                    // Add stack trace(only for development or debugging purposes)
                    //if (context.Exception != null)
                    //{
                    //    context.ProblemDetails.Extensions.Add("stackTrace", context.Exception.StackTrace);
                    //}
                };                
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(); // Document name is v1
            builder.Services.AddOpenApi("internal"); // Document name is internal

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                                   "Could not find a connection string named 'DefaultConnection'.");
            }

            // Register the ProductContext with the dependency injection container
            builder.Services.AddDbContext<ProductContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                }));

            builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register IProductRepo with its implementation
            builder.Services.AddScoped<IProductService, ProductService>(); // Register IProductService with its implementation

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(ProductMappingProfile));                       

            // Register health checks
            builder.Services.AddHealthChecks()
                .AddNpgSql(connectionString); // Add a health check for the ProductContext

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<ProductContext>();

            // Register rate limiting
            builder.Services.AddRateLimiter(options => options
                            // Define a fixed window policy
                            .AddFixedWindowLimiter(policyName: "FixedPolicy", options =>
                            {
                                options.PermitLimit = 4; // Allow 4 requests
                                options.Window = TimeSpan.FromSeconds(12); // Per 12 seconds
                                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                                options.QueueLimit = 2; // Queue 1 request if limit is exceeded
                            }).OnRejected = (context, cancellationToken) =>
                            {
                                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                                return new ValueTask();
                            });

            var app = builder.Build();

            app.UseRateLimiter();

            // Ensure database is created (apply migrations or create the schema)
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
            //    // or call dbContext.Database.Migrate() if using migrations
            //    dbContext.Database.EnsureCreated();
            //}

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
            else
            {
                // Adds a middleware to the pipeline that will catch exceptions, log them, and
                // re-execute the request in an alternate pipeline. The request will not be
                // re-executed if the response has already started.
                app.UseExceptionHandler();
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

            app.MapGet("/throw", (HttpContext context) => throw new InvalidOperationException("This is a test exception"));

            app.Run();
        }
    }
}

