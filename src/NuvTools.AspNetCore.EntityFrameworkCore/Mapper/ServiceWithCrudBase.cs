using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuvTools.AspNetCore.Mapper;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.EntityFrameworkCore.Extensions;
using System.Linq.Expressions;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Mapper;

public abstract class ServiceWithCrudBase<TContext, TDTO, TEntity, TKey>(TContext context, IMapper mapper) : ServiceWithMapperBase<TDTO, TEntity>(mapper)
                                                                where TDTO : class
                                                                where TEntity : class
                                                                where TContext : DbContext
                                                                where TKey : notnull
{
    protected readonly TContext Context = context;

    public DbSet<TEntity> Dataset { get { return Context.Set<TEntity>(); } }

    public async Task<TDTO?> FindAsync(TKey id)
    {
        return ConvertToDTO(await Context.FindAsync<TEntity>(id));
    }

    public async Task<TDTO?> FindAsync(object[] keys)
    {
        return ConvertToDTO(await Context.FindAsync<TEntity>(keys));
    }

    public async Task<IEnumerable<TDTO>?> FindFromExpressionAsync(Expression<Func<IQueryable<TEntity>>> expression)
    {
        return ConvertToDTO(await Context.FromExpression(expression).ToListAsync());
    }

    public virtual async Task<IResult<TKey>> AddAndSaveAsync(TDTO model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        return await Context.AddAndSaveAsync<TEntity, TKey>(ConvertToEntity(model));
    }

    public virtual async Task<IResult> UpdateAndSaveAsync(TKey id, TDTO model)
    {
        return await Context.UpdateAndSaveAsync(ConvertToEntity(model), id);
    }

    public virtual async Task<IResult> RemoveAndSaveAsync(TKey id)
    {
        return await Context.RemoveAndSaveAsync<TEntity>(id);
    }
}
