using AutoMapper;

namespace NuvTools.AspNetCore.Mapper;

public abstract class ServiceWithMapperBase<TForm, TData> where TForm : class
                                                    where TData : class
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

    protected TReturn ConvertTo<TReturn>(TData model)
    {
        return _mapper.Map<TReturn>(model);
    }

    protected IEnumerable<TReturn> ConvertTo<TReturn>(IEnumerable<TData> models)
    {
        return _mapper.Map<IEnumerable<TReturn>>(models);
    }

    protected TData ConvertToData(TForm model)
    {
        return _mapper.Map<TData>(model);
    }

    protected TForm ConvertToForm(TData model)
    {
        return _mapper.Map<TForm>(model);
    }

    protected IEnumerable<TForm> ConvertToForm(IEnumerable<TData> models)
    {
        return _mapper.Map<IEnumerable<TForm>>(models);
    }

    protected IList<TForm> ConvertToForm(IList<TData> models)
    {
        return _mapper.Map<IList<TForm>>(models);
    }

    protected TForm[] ConvertToForm(TData[] models)
    {
        return _mapper.Map<TForm[]>(models);
    }

    #endregion

}
