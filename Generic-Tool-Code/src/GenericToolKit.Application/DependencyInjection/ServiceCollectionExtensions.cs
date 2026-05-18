using GenericToolKit.Application.Services;
using GenericToolKit.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GenericToolKit.Application.DependencyInjection
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddGenericService<TEntity>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : BaseEntity
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(IGenericService<TEntity>),
                    typeof(GenericService<TEntity>),
                    lifetime));

            return services;
        }

        // Adds generic services
        public static IServiceCollection AddGenericServices(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var entityTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t)
                         && !t.IsAbstract
                         && !t.IsInterface)
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var serviceInterface = typeof(IGenericService<>).MakeGenericType(entityType);
                var serviceImplementation = typeof(GenericService<>).MakeGenericType(entityType);

                services.Add(
                    new ServiceDescriptor(
                        serviceInterface,
                        serviceImplementation,
                        lifetime));
            }

            return services;
        }

        // Adds generic services from assembly
        public static IServiceCollection AddGenericServicesFromAssembly(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var assembly = Assembly.GetCallingAssembly();
            return services.AddGenericServices(new[] { assembly }, lifetime);
        }
    }
}

