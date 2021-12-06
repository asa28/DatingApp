using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
  public static class DependencyInjectionConfigurationService
  {
    public static IServiceCollection AddDependencyInjectionConfigurationService(this IServiceCollection services, IConfiguration config)
    {
      // Injected Services Used 
      services.AddScoped<ITokenService, TokenService>();


      // DbContext injected
      services.AddDbContext<DataContext>(opts =>
      {
        opts.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      return services;
    }
  }
}