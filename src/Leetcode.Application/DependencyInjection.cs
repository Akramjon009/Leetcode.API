using Leetcode.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Leetcode.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICompilerService, CompilerService>();

            return services;
        }
    }
}
