using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuvTools.AspNetCore.Mapper;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.EntityFrameworkCore.Extensions;
using System.Linq.Expressions;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Mapper;

/// <summary>
/// Provides a base class for service classes that require CRUD (Create, Read, Update, Delete) operations
/// with Entity Framework Core and AutoMapper integration.
/// </summary>
/// <typeparam name="TContext">The type of the Entity Framework <see cref="DbContext"/>.</typeparam>
/// <typeparam name="TDTO">The type of the Data Transfer Object (DTO).</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
/// <remarks>
/// This class extends <see cref="ServiceWithMapperBase{TDTO, TEntity}"/> and adds database operation capabilities
/// including finding, adding, updating, and removing entities with automatic DTO-to-entity conversion.
/// </remarks>
public abstract class ServiceWithCrudBase<TContext, TDTO, TEntity, TKey>(TContext context, IMapper mapper) : ServiceWithMapperBase<TDTO, TEntity>(mapper)
                                                                where TDTO : class
                                                                where TEntity : class
                                                                where TContext : DbContext
                                                                where TKey : notnull
{
    /// <summary>
    /// Gets the Entity Framework <see cref="DbContext"/> instance.
    /// </summary>
    protected readonly TContext Context = context;

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for the entity type.
    /// </summary>
    public DbSet<TEntity> Dataset { get { return Context.Set<TEntity>(); } }

    /// <summary>
    /// Finds an entity by its primary key asynchronously and converts it to a DTO.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the DTO
    /// if the entity is found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<TDTO?> FindAsync(TKey id)
    {
        return ConvertToDTO(await Context.FindAsync<TEntity>(id));
    }

    /// <summary>
    /// Finds an entity by its composite key asynchronously and converts it to a DTO.
    /// </summary>
    /// <param name="keys">An array of key values representing a composite key.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the DTO
    /// if the entity is found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<TDTO?> FindAsync(object[] keys)
    {
        return ConvertToDTO(await Context.FindAsync<TEntity>(keys));
    }

    /// <summary>
    /// Finds entities using a LINQ expression asynchronously and converts them to DTOs.
    /// </summary>
    /// <param name="expression">An expression that returns an <see cref="IQueryable{TEntity}"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of DTOs,
    /// or <c>null</c> if no entities are found.
    /// </returns>
    public async Task<IEnumerable<TDTO>?> FindFromExpressionAsync(Expression<Func<IQueryable<TEntity>>> expression)
    {
        return ConvertToDTO(await Context.FromExpression(expression).ToListAsync());
    }

    /// <summary>
    /// Adds a new entity to the database and saves changes asynchronously.
    /// </summary>
    /// <param name="model">The DTO representing the entity to add.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult{TKey}"/>
    /// with the primary key of the added entity if successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
    public virtual async Task<IResult<TKey>> AddAndSaveAsync(TDTO model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        return await Context.AddAndSaveAsync<TEntity, TKey>(ConvertToEntity(model));
    }

    /// <summary>
    /// Updates an existing entity in the database and saves changes asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the entity to update.</param>
    /// <param name="model">The DTO containing the updated values.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating the success or failure of the operation.
    /// </returns>
    public virtual async Task<IResult> UpdateAndSaveAsync(TKey id, TDTO model)
    {
        return await Context.UpdateAndSaveAsync(ConvertToEntity(model), id);
    }

    /// <summary>
    /// Removes an entity from the database and saves changes asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the entity to remove.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating the success or failure of the operation.
    /// </returns>
    public virtual async Task<IResult> RemoveAndSaveAsync(TKey id)
    {
        return await Context.RemoveAndSaveAsync<TEntity>(id);
    }
}
