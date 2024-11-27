using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Models;
using CollabParty.Infrastructure.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CollabParty.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddTransient<IEmailService, SendGridEmailService>();

            return services;
        }
    }
}