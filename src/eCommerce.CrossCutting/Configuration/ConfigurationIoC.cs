using System.Data;
using System.Data.SqlClient;
using eCommerce.Domain.Models.Interfaces.Repository;
using eCommerce.Domain.Models.Interfaces.Repository.Email;
using eCommerce.Domain.Models.Interfaces.Service;
using eCommerce.Infrastructure.Repository;
using eCommerce.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.CrossCutting.Configuration
{
    public class ConfigurationIoC
    {
        public static void Configure(IServiceCollection service)
        {
            //Repository
            service.AddScoped<IUsuarioRepository, UsuarioRepository>();
            service.AddScoped<IEmailRepository, EmailRepository>();

            
            //Service
            service.AddScoped<IUsuarioService, UsuarioService>();
            
            
            //Connection String
            service.AddTransient<IDbConnection>(sp => new SqlConnection("Server=localhost\\SQLEXPRESS;Database=eCommerce;Trusted_Connection=True;"));
        }
    }
}