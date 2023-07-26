using Microsoft.EntityFrameworkCore;
using Taktamir.Framework.Utilities;
using System.Linq.Expressions;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.infra.Data.sql._01.Common
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly AppDbContext DbContext;
        public DbSet<TEntity> Entities { get; }
        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public Repository(AppDbContext dbContext)
        {
            DbContext = dbContext;
            Entities = DbContext.Set<TEntity>(); // City => Cities
        }

        #region Async Method

        public virtual async ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await Entities.FindAsync(ids, cancellationToken);
                    transaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                { 
                    await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    Entities.Update(entity);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    Entities.UpdateRange(entities);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    Entities.Remove(entity);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    Entities.RemoveRange(entities);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        #endregion

        #region Sync Methods
        public virtual TEntity GetById(params object[] ids)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var result = Entities.Find(ids);
                    transaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void Add(TEntity entity, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    Entities.Add(entity);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    Entities.AddRange(entities);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    Entities.Update(entity);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    Entities.UpdateRange(entities);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    Entities.Remove(entity);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entities, nameof(entities));
                    Entities.RemoveRange(entities);
                    if (saveNow)
                        DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region Attach & Detach
        public virtual void Detach(TEntity entity)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    var entry = DbContext.Entry(entity);
                    if (entry != null)
                        entry.State = EntityState.Detached;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void Attach(TEntity entity)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Assert.NotNull(entity, nameof(entity));
                    if (DbContext.Entry(entity).State == EntityState.Detached)
                        Entities.Attach(entity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Explicit Loading


        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
        where TProperty : class
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Attach(entity);
                    var collection = DbContext.Entry(entity).Collection(collectionProperty);
                    if (!collection.IsLoaded)
                        await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
        where TProperty : class
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Attach(entity);
                    var collection = DbContext.Entry(entity).Collection(collectionProperty);
                    if (!collection.IsLoaded)
                        collection.Load();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
        where TProperty : class
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Attach(entity);
                    var reference = DbContext.Entry(entity).Reference(referenceProperty);
                    if (!reference.IsLoaded)
                        await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
        where TProperty : class
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Attach(entity);
                    var reference = DbContext.Entry(entity).Reference(referenceProperty);
                    if (!reference.IsLoaded)
                        reference.Load();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion
    }

}
