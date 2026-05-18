using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Extensions;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Domain.Models;
using GenericToolKit.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace GenericToolKit.Application.Services
{

    public class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        private readonly IGenericRepository<T> baseRepository;
        private readonly ILoggedInUser loggedInUser;
        private const string LayerName = "GenericService";

        // Initializes the generic application service
        public GenericService(IGenericRepository<T> baseRepository, ILoggedInUser loggedInUser)
        {
            this.baseRepository = baseRepository ?? throw new ArgumentNullException(nameof(baseRepository));
            this.loggedInUser = loggedInUser ?? throw new ArgumentNullException(nameof(loggedInUser));
        }

        // Adds
        public async Task<T> Add(T entity)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return null;
                    return await this.baseRepository.Add(entity);
                },
                nameof(Add),
                LayerName,
                defaultValue: null);
        }

        // Adds many
        public async Task<bool> AddMany(IEnumerable<T> entities)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (!entities.IsValidEnumerable())
                        return false;
                    return await this.baseRepository.AddMany(entities);
                },
                nameof(AddMany),
                LayerName,
                defaultValue: false);
        }

        // Checks if any entity matches
        public async Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (predicate is null)
                        return false;
                    return await this.baseRepository.Any(predicate, cancellationToken);
                },
                nameof(Any),
                LayerName,
                defaultValue: false);
        }

        // Counts
        public async Task<int> Count(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (predicate is null)
                        return 0;
                    return await this.baseRepository.Count(predicate, cancellationToken);
                },
                nameof(Count),
                LayerName,
                defaultValue: 0);
        }

        // Commits transaction
        public async Task<bool> CommitTransactionAsync(IDbContextTransaction transaction, bool shouldCommit)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (transaction == null)
                        throw new ArgumentNullException(nameof(transaction));
                    return await this.baseRepository.CommitTransactionAsync(transaction, shouldCommit);
                },
                nameof(CommitTransactionAsync),
                LayerName,
                defaultValue: false);
        }

        // Detects change
        public async Task<string> DetectChange(T entity)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return string.Empty;
                    return await this.baseRepository.DetectChange(entity);
                },
                nameof(DetectChange),
                LayerName,
                defaultValue: string.Empty);
        }

        // Finds
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, BaseFilters? findOptions)
        {
            return ExceptionHandler.Execute(
                () =>
                {
                    if (predicate is null)
                        return Enumerable.Empty<T>().AsQueryable();
                    return this.baseRepository.Find(predicate, findOptions);
                },
                nameof(Find),
                LayerName,
                defaultValue: Enumerable.Empty<T>().AsQueryable());
        }

        // Finds one
        public async Task<T?> FindOne(Expression<Func<T, bool>> predicate, BaseFilters? findOptions)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (predicate is null)
                        return null;
                    return await this.baseRepository.FindOne(predicate, findOptions);
                },
                nameof(FindOne),
                LayerName,
                defaultValue: null);
        }

        // Gets all
        public async Task<List<T>> GetAll(BaseFilters? filters)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    var result = await this.baseRepository.GetAll(filters);
                    return result ?? new List<T>();
                },
                nameof(GetAll),
                LayerName,
                defaultValue: new List<T>());
        }

        // Gets by id query
        public IQueryable<T> GetByIdQuery(int id, bool detached)
        {
            return ExceptionHandler.Execute(
                () =>
                {
                    if (id <= 0)
                        return Enumerable.Empty<T>().AsQueryable();
                    return this.baseRepository.GetById(id, detached);
                },
                nameof(GetByIdQuery),
                LayerName,
                defaultValue: Enumerable.Empty<T>().AsQueryable());
        }

        // Hard-deletes delete by id
        public async Task<bool> HardDeleteById(int id)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (id <= 0)
                        return false;
                    return await this.baseRepository.HardDeleteById(id);
                },
                nameof(HardDeleteById),
                LayerName,
                defaultValue: false);
        }

        // Hard-deletes delete many
        public async Task<int> HardDeleteMany(Expression<Func<T, bool>> predicate)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (predicate is null)
                        return 0;
                    return await this.baseRepository.HardDeleteMany(predicate);
                },
                nameof(HardDeleteMany),
                LayerName,
                defaultValue: 0);
        }

        // Hard-deletes delete one
        public async Task<int> HardDeleteOne(T entity)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return 0;
                    return await this.baseRepository.HardDeleteOne(entity);
                },
                nameof(HardDeleteOne),
                LayerName,
                defaultValue: 0);
        }

        // Lists
        public async Task<List<T>> ListAsync(List<int> Ids, CancellationToken cancellationToken)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (!Ids.IsValidList())
                        return new List<T>();
                    return await this.baseRepository.ListAsync(Ids, cancellationToken);
                },
                nameof(ListAsync),
                LayerName,
                defaultValue: new List<T>());
        }

        // Lists by specs
        public async Task<List<T>> ListBySpecs(IBaseSpecification<T> specification, CancellationToken cancellationToken)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (specification is null)
                        return new List<T>();
                    var result = await this.baseRepository.ListBySpecs(specification, cancellationToken);
                    return result ?? new List<T>();
                },
                nameof(ListBySpecs),
                LayerName,
                defaultValue: new List<T>());
        }

        // Logs full json comparison
        public async Task<string> LogFullJsonComparison(T entity)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return string.Empty;
                    return await this.baseRepository.LogFullJsonComparison(entity);
                },
                nameof(LogFullJsonComparison),
                LayerName,
                defaultValue: string.Empty);
        }

        // Removes list of entities
        public async Task<bool> RemoveListOfEntities(List<T> entities)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (!entities.IsValidList())
                        return false;
                    return await this.baseRepository.RemoveListOfEntities(entities);
                },
                nameof(RemoveListOfEntities),
                LayerName,
                defaultValue: false);
        }

        // Restores original values
        public async Task<T> RestoreOriginalValuesAsync(T entityToUpdate, List<string> propertiesToUpdate)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entityToUpdate is null || !propertiesToUpdate.IsValidList())
                        return null;
                    return await this.baseRepository.RestoreOriginalValuesAsync(entityToUpdate, propertiesToUpdate);
                },
                nameof(RestoreOriginalValuesAsync),
                LayerName,
                defaultValue: null);
        }

        // Rolls back transaction
        public async Task<bool> RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (transaction == null)
                        throw new ArgumentNullException(nameof(transaction));
                    return await this.baseRepository.RollbackTransactionAsync(transaction);
                },
                nameof(RollbackTransactionAsync),
                LayerName,
                defaultValue: false);
        }

        // Saves or updates or update
        public async Task<T> SaveOrUpdate(T entity, bool setAuditProperties, bool shouldSave)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return null;
                    return await this.baseRepository.SaveOrUpdate(entity, setAuditProperties, shouldSave);
                },
                nameof(SaveOrUpdate),
                LayerName,
                defaultValue: null);
        }

        // Sets audit properties
        public async Task<T> SetAuditPropertiesAsync(T entity)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return null;
                    await this.baseRepository.SetAuditProperties(entity);
                    return entity;
                },
                nameof(SetAuditPropertiesAsync),
                LayerName,
                defaultValue: null);
        }

        // Soft-deletes delete many
        public async Task<bool> SoftDeleteMany(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (!entities.IsValidEnumerable())
                        return false;
                    return await this.baseRepository.SoftDeleteMany(entities, cancellationToken);
                },
                nameof(SoftDeleteMany),
                LayerName,
                defaultValue: false);
        }

        // Soft-deletes delete one
        public async Task<bool> SoftDeleteOne(T entity, CancellationToken cancellationToken = default)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return false;
                    return await this.baseRepository.SoftDeleteOne(entity, cancellationToken);
                },
                nameof(SoftDeleteOne),
                LayerName,
                defaultValue: false);
        }

        // Starts transaction
        public async Task<IDbContextTransaction> StartTransaction()
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    return await this.baseRepository.StartTransaction();
                },
                nameof(StartTransaction),
                LayerName,
                defaultValue: null);
        }

        // Updates one
        public async Task<T> UpdateOne(T entity, CancellationToken token)
        {
            return await ExceptionHandler.ExecuteAsync(
                async () =>
                {
                    if (entity is null)
                        return null;
                    await this.baseRepository.UpdateOne(entity, token);
                    return await this.GetByIdQuery(entity.Id, true).FirstOrDefaultAsync();
                },
                nameof(UpdateOne),
                LayerName,
                defaultValue: null);
        }
    }
}

