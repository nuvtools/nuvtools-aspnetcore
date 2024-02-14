using AutoMapper;

namespace NuvTools.AspNetCore.Mapper;

public abstract class ServiceWithMapperBase<TDTO, TEntity> where TDTO : class
                                                    where TEntity : class
{
    private readonly IMapper _mapper;

    protected IMapper Mapper
    {
        get
        {
            return _mapper;
        }
    }

    public ServiceWithMapperBase(IMapper mapper)
    {
        _mapper = mapper;
    }

    #region Converter

    protected TReturn ConvertTo<TReturn>(TEntity model)
    {
        return _mapper.Map<TReturn>(model);
    }

    protected IEnumerable<TReturn> ConvertTo<TReturn>(IEnumerable<TEntity> models)
    {
        return _mapper.Map<IEnumerable<TReturn>>(models);
    }

    protected TEntity ConvertToEntity(TDTO model)
    {
        return _mapper.Map<TEntity>(model);
    }

    protected TDTO ConvertToDTO(TEntity model)
    {
        return _mapper.Map<TDTO>(model);
    }

    protected IEnumerable<TDTO> ConvertToDTO(IEnumerable<TEntity> models)
    {   
        return _mapper.Map<IEnumerable<TDTO>>(models);
    }

    protected IList<TDTO> ConvertToDTO(IList<TEntity> models)
    {
        return _mapper.Map<IList<TDTO>>(models);
    }

    protected TDTO[] ConvertToDTO(TEntity[] models)
    {
        return _mapper.Map<TDTO[]>(models);
    }

    #endregion

}
