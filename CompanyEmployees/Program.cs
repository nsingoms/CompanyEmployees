
using CompanyEmployees.Extentions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;

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
         builder.Services.ConfigureSqlContext(builder.Configuration);
         builder.Services.Configure<ApiBehaviorOptions>(options =>
         {
            options.SuppressModelStateInvalidFilter = true;
         });

         builder.Services.AddControllers(config =>
               {
                  config.RespectBrowserAcceptHeader = true;
                  config.ReturnHttpNotAcceptable = true;
               }).AddXmlDataContractSerializerFormatters()
                 .AddCustomCSVFormatter()
                 .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
         builder.Services.AddAutoMapper(typeof(Program));


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
         app.UseCors("CorsPolicy");
         app.UseAuthorization();


         app.MapControllers();

         app.Run();
      }
   }
}