using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace GenericToolKit.Application.Services
{

    public interface ITransactionService
    {
        // Commits transaction
        Task<bool> CommitTransactionAsync(IDbContextTransaction transaction, bool shouldCommit);
        // Rolls back transaction
        Task<bool> RollbackTransactionAsync(IDbContextTransaction transaction);
        // Starts transaction
        Task<IDbContextTransaction> StartTransaction();
    }

    public interface ICrudService<T> where T : BaseEntity
    {
        // Adds
        Task<T> Add(T entity);
        // Gets by id query
        IQueryable<T> GetByIdQuery(int id, bool detached = true);
        // Hard-deletes delete by id
        Task<bool> HardDeleteById(int id);
        // Hard-deletes delete many
        Task<int> HardDeleteMany(Expression<Func<T, bool>> predicate);
        // Hard-deletes delete one
        Task<int> HardDeleteOne(T entity);
        // Saves or updates or update
        Task<T> SaveOrUpdate(T entity, bool setAuditProperties = true, bool shouldSave = true);
        // Soft-deletes delete many
        Task<bool> SoftDeleteMany(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        // Soft-deletes delete one
        Task<bool> SoftDeleteOne(T entity, CancellationToken cancellationToken = default);
        // Updates one
        Task<T> UpdateOne(T entity, CancellationToken token);
    }

    public interface IQueryService<T> where T : BaseEntity
    {
        // Gets all
        Task<List<T>> GetAll(BaseFilters? filters = null);
        // Finds one
        Task<T?> FindOne(Expression<Func<T, bool>> predicate, BaseFilters? findOptions = null);
        // Finds
        IQueryable<T> Find(Expression<Func<T, bool>> predicate, BaseFilters? findOptions = null);
        // Lists
        Task<List<T>> ListAsync(List<int> Ids, CancellationToken cancellationToken = default);
        // Checks if any entity matches
        Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        // Counts
        Task<int> Count(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        // Lists by specs
        Task<List<T>> ListBySpecs(IBaseSpecification<T> specification, CancellationToken cancellationToken = default);
    }

    public interface IChangeTrackingService<T> where T : BaseEntity
    {
        // Detects change
        Task<string> DetectChange(T entity);
        // Logs full json comparison
        Task<string> LogFullJsonComparison(T entity);
    }

    public interface IAuditService<T> where T : BaseEntity
    {
        // Sets audit properties
        Task<T> SetAuditPropertiesAsync(T entity);
    }

    public interface IRemovalService<T> where T : BaseEntity
    {
        // Removes list of entities
        Task<bool> RemoveListOfEntities(List<T> entities);
    }

    public interface IAdditionalService<T> where T : BaseEntity
    {
        // Restores original values
        Task<T> RestoreOriginalValuesAsync(T entityToUpdate, List<string> propertiesToUpdate);
        // Adds many
        Task<bool> AddMany(IEnumerable<T> entities);
    }

    public interface IGenericService<T> :
        ITransactionService,
        ICrudService<T>,
        IQueryService<T>,
        IChangeTrackingService<T>,
        IAuditService<T>,
        IRemovalService<T>,
        IAdditionalService<T>
        where T : BaseEntity
    {
    }
}

