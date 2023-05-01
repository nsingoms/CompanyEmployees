using AspNetCoreRateLimit;
using CompanyEmployees.Extentions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Service.DataShaping;
using Shared.DataTransferObjects;

namespace CompanyEmployees
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var builder = WebApplication.CreateBuilder(args);
         LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
                  "/nlog.config"));
         // Add services to the container.
         builder.Services.ConfigureCors();
         builder.Services.ConfigureIISConfiguration();
         builder.Services.ConfigureLoggerService();
         builder.Services.ConfigureRepositoryManager();
         builder.Services.ConfigureServiceManager();
         builder.Services.ConfigureVersioning();
         builder.Services.AddMemoryCache();
         builder.Services.ConfigureResponseCaching();
         builder.Services.ConfigureHttpCacheHeaders();
         builder.Services.ConfigureRateLimitingOptions();
         builder.Services.AddHttpContextAccessor();
         builder.Services.AddAuthentication();
         builder.Services.ConfigureIdentity();
         builder.Services.ConfigureJWT(builder.Configuration);

         builder.Services.ConfigureSqlContext(builder.Configuration); builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
         builder.Services.AddScoped<ValidateMediaTypeAttribute>();
         builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
         builder.Services.Configure<ApiBehaviorOptions>(options =>
         {
            options.SuppressModelStateInvalidFilter = true;
         });
         builder.Services.AddScoped<ValidationFilterAttribute>();

         builder.Services.AddControllers(config =>
               {
                  config.RespectBrowserAcceptHeader = true;
                  config.ReturnHttpNotAcceptable = true;
                  config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                  {
                     Duration =
                  120
                  });
               }).AddXmlDataContractSerializerFormatters()
                 .AddCustomCSVFormatter()
                 .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
         builder.Services.AddAutoMapper(typeof(Program));
         builder.Services.AddCustomMediaTypes();
         builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

         var app = builder.Build();


         var logger = app.Services.GetRequiredService<ILoggerManager>();
         app.ConfigureExceptionHandler(logger);
         if (app.Environment.IsProduction())
            app.UseHsts();

         app.UseHttpsRedirection();
         app.UseStaticFiles();
         app.UseForwardedHeaders(new ForwardedHeadersOptions
         {
            ForwardedHeaders = ForwardedHeaders.All
         });
         app.UseIpRateLimiting();

         app.UseCors("CorsPolicy");
         app.UseAuthentication();
         app.UseAuthorization();

         app.UseResponseCaching();
         app.UseHttpCacheHeaders();
         app.MapControllers();

         app.Run();
      }
   }
}