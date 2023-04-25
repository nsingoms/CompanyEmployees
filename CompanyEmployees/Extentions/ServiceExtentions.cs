using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

namespace CompanyEmployees.Extentions;

public static class ServiceExtentions
{
   public static void ConfigureCors(this IServiceCollection services)
   {
      services.AddCors(options =>
      {
         options.AddPolicy("MyPolicy", builder =>
                                     builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .WithExposedHeaders("X-Pagination"));

   });

   }
   public static void ConfigureIISConfiguration(this IServiceCollection services)
   {
      services.Configure<IISOptions>(options => { });
   }
   public static void ConfigureLoggerService(this IServiceCollection services) =>
    services.AddSingleton<ILoggerManager, LoggerManager>();
   public static void ConfigureRepositoryManager(this IServiceCollection services) =>
      services.AddScoped<IRepositoryManager, RepositoryManager>();
   public static void ConfigureServiceManager(this IServiceCollection services) =>
      services.AddScoped<IServiceManager, ServiceManager>();
   public static void ConfigureSqlContext(this IServiceCollection services,
      IConfiguration configuration) => services.AddDbContext<RepositoryContext>(opts =>
      opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
   public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
    builder.AddMvcOptions(config => config.OutputFormatters.Add(new
   CsvOutputFormatter()));

}
