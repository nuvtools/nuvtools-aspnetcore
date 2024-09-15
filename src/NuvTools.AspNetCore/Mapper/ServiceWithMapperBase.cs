using AutoMapper;

namespace NuvTools.AspNetCore.Mapper;

public abstract class ServiceWithMapperBase<TDTO, TEntity>(IMapper mapper) where TDTO : class
                                                    where TEntity : class
{
    protected IMapper Mapper
    {
        get
        {
            return mapper;
        }
    }

    #region Converter

    protected TReturn? ConvertTo<TReturn>(TEntity? model)
    {
        if (model is null) return default;
        return mapper.Map<TReturn>(model);
    }

    protected IEnumerable<TReturn>? ConvertTo<TReturn>(IEnumerable<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IEnumerable<TReturn>>(models);
    }

    protected TEntity ConvertToEntity(TDTO model)
    {
        return mapper.Map<TEntity>(model);
    }

    protected TDTO? ConvertToDTO(TEntity? model)
    {
        if (model is null) return null;
        return mapper.Map<TDTO>(model);
    }

    protected IEnumerable<TDTO>? ConvertToDTO(IEnumerable<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IEnumerable<TDTO>>(models);
    }

    protected IList<TDTO>? ConvertToDTO(IList<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IList<TDTO>>(models);
    }

    protected TDTO[]? ConvertToDTO(TEntity[]? models)
    {
        if (models is null) return null;
        return mapper.Map<TDTO[]>(models);
    }

    #endregion

}
