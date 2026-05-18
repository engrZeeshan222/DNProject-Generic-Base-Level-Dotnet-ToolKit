using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace GenericToolKit.Domain.Interfaces
{

    public interface ITransactionRepository<T> where T : BaseEntity
    {
        // Commits transaction
        Task<bool> CommitTransactionAsync(IDbContextTransaction transaction, bool shouldCommit);
        // Rolls back transaction
        Task<bool> RollbackTransactionAsync(IDbContextTransaction transaction);
        // Starts transaction
        Task<IDbContextTransaction> StartTransaction();
    }

    public interface IEntityCrudRepository<T> where T : BaseEntity
    {
        // Adds
        Task<T> Add(T entity);
        // Gets by id
        IQueryable<T> GetById(int id, bool detached = true);
        // Hard-deletes delete by id
        Task<bool> HardDeleteById(int Id);
        // Hard-deletes delete many
        Task<int> HardDeleteMany(Expression<Func<T, bool>> predicate);
        // Hard-deletes delete one
        Task<int> HardDeleteOne(T entity);
        // Saves or updates or update
        Task<T> SaveOrUpdate(T entity, bool setAuditProperties = true, bool shouldSave = true);
        // Sets entity state recursively n upsert multiple
        Task<bool> SetEntityStateRecursively_N_UpsertMultiple(List<T> entities, CancellationToken cancellationToken = default);
        // Soft-deletes delete many
        Task<bool> SoftDeleteMany(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        // Soft-deletes delete many by conditions
        Task<bool> SoftDeleteManyByConditions(Expression<Func<T, bool>> predicates, CancellationToken cancellationToken = default);
        // Soft-deletes delete one
        Task<bool> SoftDeleteOne(T entity, CancellationToken cancellationToken = default);
        // Updates one
        Task UpdateOne(T entity, CancellationToken token);
    }

    public interface IEntityQueryRepository<T> where T : BaseEntity
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
        IQueryable<TResult> ProjectableListBySpecs<TResult>(IProjectableSpecifications<T, TResult> specification, CancellationToken cancellationToken = default) where TResult : BaseInOutDTO;
    }

    public interface IEntityChangeTrackingRepository<T> where T : BaseEntity
    {
        // Detects change
        Task<string> DetectChange(T entity);
        // Logs full json comparison
        Task<string> LogFullJsonComparison(T entity);
        // Creates return base entry object
        Task<BaseEntry<T>> CreateReturnBaseEntryObject(EntityEntry entry);
        // Gets modified properties as dictionary
        Dictionary<string, object> GetModifiedPropertiesAsDictionary(BaseEntry<T> trackedEntry);
        // Adds or attach entity
        EntityEntry AddOrAttachEntity(T entity);
        // Extracts modified only old properties
        Dictionary<string, object> ExtractModifiedOnlyOldProperties(BaseEntry<T> entry);
        // Extracts modified only changed properties
        Dictionary<string, object> ExtractModifiedOnlyChangedProperties(BaseEntry<T> entry);
    }

    public interface IAuditRepository<T> where T : BaseEntity
    {
        // Sets audit properties
        Task SetAuditProperties(T entity);
    }

    public interface IEntityRemovalRepository<T> where T : BaseEntity
    {
        // Removes list of entities
        Task<bool> RemoveListOfEntities(List<T> entities);
    }

    public interface IGenericRepository<T> :
        IEntityChangeTrackingRepository<T>,
        IAuditRepository<T>,
        ITransactionRepository<T>,
        IEntityRemovalRepository<T>,
        IEntityCrudRepository<T>,
        IEntityQueryRepository<T>
        where T : BaseEntity
    {
        // Restores original values
        Task<T> RestoreOriginalValuesAsync(T entityToUpdate, List<string> propertiesToUpdate);
        // Adds many
        Task<bool> AddMany(IEnumerable<T> entities);
    }
}

