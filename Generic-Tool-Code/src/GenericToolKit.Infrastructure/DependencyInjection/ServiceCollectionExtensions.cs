using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Infrastructure.Data;
using GenericToolKit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GenericToolKit.Infrastructure.DependencyInjection
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddBaseContext<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped
            )
            where TContext : BaseContext
        {
            if (optionsAction != null)
            {
                services.AddDbContext<TContext>(optionsAction, contextLifetime);
            }
            else
            {
                services.AddDbContext<TContext>(contextLifetime);
            }
            return services;
        }
        
        public static IServiceCollection AddSingleGenericRepository<TEntity>(this IServiceCollection services,ServiceLifetime lifetime = ServiceLifetime.Scoped) where TEntity : BaseEntity
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(IGenericRepository<TEntity>),
                    typeof(GenericRepository<TEntity>),
                    lifetime));

            return services;
        }

        // Adds generic repositories
        public static IServiceCollection AddMultipleGenReposFromAssembly(
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
                var repositoryInterface = typeof(IGenericRepository<>).MakeGenericType(entityType);
                var repositoryImplementation = typeof(GenericRepository<>).MakeGenericType(entityType);

                services.Add(
                    new ServiceDescriptor(
                        repositoryInterface,
                        repositoryImplementation,
                        lifetime));
            }

            return services;
        }

    }
}

