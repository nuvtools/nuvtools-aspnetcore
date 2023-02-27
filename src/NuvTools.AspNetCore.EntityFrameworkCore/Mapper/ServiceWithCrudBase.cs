using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuvTools.AspNetCore.Mapper;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NuvTools.AspNetCore.EntityFrameworkCore.Mapper;

public abstract class ServiceWithCrudBase<TContext, TForm, TData, TKey> : ServiceWithMapperBase<TForm, TData>
                                                                where TForm : class
                                                                where TData : class
                                                                where TContext : DbContext
{
    protected readonly TContext Context;

    public ServiceWithCrudBase(TContext context, IMapper mapper) : base(mapper)
    {
        Context = context;
    }

    public DbSet<TData> Dataset { get { return Context.Set<TData>(); } }

    public async Task<TForm> FindAsync(TKey id)
    {
        return ConvertToForm(await Context.FindAsync<TData>(id));
    }

    public async Task<TForm> FindAsync(object[] keys)
    {
        return ConvertToForm(await Context.FindAsync<TData>(keys));
    }

    public async Task<IEnumerable<TForm>> FindFromExpressionAsync(Expression<Func<IQueryable<TData>>> expression)
    {
        return ConvertToForm(await Context.FromExpression(expression).ToListAsync());
    }

    public virtual async Task<IResult<TKey>> AddAndSaveAsync(TForm model)
    {
        return await Context.AddAndSaveAsync<TData, TKey>(ConvertToData(model));
    }

    public virtual async Task<IResult> UpdateAndSaveAsync(TKey id, TForm model)
    {
        return await Context.UpdateAndSaveAsync(ConvertToData(model), id);
    }

    public virtual async Task<IResult> RemoveAndSaveAsync(TKey id)
    {
        return await Context.RemoveAndSaveAsync<TData>(id);
    }
}
