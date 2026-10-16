using LibraryLab.Application.Contracts;
using LibraryLab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryLabInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LibraryLab")
            ?? "Server=localhost;Port=3306;Database=library_lab;User=library_lab;Password=library_lab;";

        // La versión del servidor se detecta automáticamente (MySQL o MariaDB). Si se indica
        // explícitamente Database:ServerVersion, se usa ese valor; si el servidor no está
        // disponible en este momento (por ejemplo, en pruebas con repositorios en memoria),
        // se usa una versión MySQL 8 compatible como respaldo.
        var configuredVersion = configuration["Database:ServerVersion"];
        ServerVersion serverVersion;
        if (!string.IsNullOrWhiteSpace(configuredVersion))
        {
            serverVersion = new MySqlServerVersion(Version.Parse(configuredVersion));
        }
        else
        {
            try
            {
                serverVersion = ServerVersion.AutoDetect(connectionString);
            }
            catch
            {
                serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
            }
        }

        services.AddDbContext<LibraryLabDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));
        services.AddScoped<ILibraryUserRepository, LibraryUserRepository>();
        services.AddScoped<ILibraryResourceRepository, LibraryResourceRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }
}
