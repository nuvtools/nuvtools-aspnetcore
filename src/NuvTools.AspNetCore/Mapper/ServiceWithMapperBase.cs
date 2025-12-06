using AutoMapper;

namespace NuvTools.AspNetCore.Mapper;

/// <summary>
/// Provides a base class for service classes that require AutoMapper functionality
/// for converting between DTOs and entities.
/// </summary>
/// <typeparam name="TDTO">The type of the Data Transfer Object (DTO).</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <remarks>
/// This class provides protected helper methods for common mapping operations,
/// simplifying conversion between DTOs and entities in derived service classes.
/// </remarks>
public abstract class ServiceWithMapperBase<TDTO, TEntity>(IMapper mapper) where TDTO : class
                                                    where TEntity : class
{
    /// <summary>
    /// Gets the AutoMapper instance used for object mapping.
    /// </summary>
    protected IMapper Mapper
    {
        get
        {
            return mapper;
        }
    }

    #region Converter

    /// <summary>
    /// Converts an entity to a specified return type using AutoMapper.
    /// </summary>
    /// <typeparam name="TReturn">The type to convert to.</typeparam>
    /// <param name="model">The entity to convert.</param>
    /// <returns>The converted object, or <c>null</c> if the input is <c>null</c>.</returns>
    protected TReturn? ConvertTo<TReturn>(TEntity? model)
    {
        if (model is null) return default;
        return mapper.Map<TReturn>(model);
    }

    /// <summary>
    /// Converts a collection of entities to a collection of the specified return type using AutoMapper.
    /// </summary>
    /// <typeparam name="TReturn">The type to convert to.</typeparam>
    /// <param name="models">The collection of entities to convert.</param>
    /// <returns>The converted collection, or <c>null</c> if the input is <c>null</c>.</returns>
    protected IEnumerable<TReturn>? ConvertTo<TReturn>(IEnumerable<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IEnumerable<TReturn>>(models);
    }

    /// <summary>
    /// Converts a DTO to an entity using AutoMapper.
    /// </summary>
    /// <param name="model">The DTO to convert.</param>
    /// <returns>The converted entity.</returns>
    protected TEntity ConvertToEntity(TDTO model)
    {
        return mapper.Map<TEntity>(model);
    }

    /// <summary>
    /// Converts an entity to a DTO using AutoMapper.
    /// </summary>
    /// <param name="model">The entity to convert.</param>
    /// <returns>The converted DTO, or <c>null</c> if the input is <c>null</c>.</returns>
    protected TDTO? ConvertToDTO(TEntity? model)
    {
        if (model is null) return null;
        return mapper.Map<TDTO>(model);
    }

    /// <summary>
    /// Converts a collection of entities to a collection of DTOs using AutoMapper.
    /// </summary>
    /// <param name="models">The collection of entities to convert.</param>
    /// <returns>The converted collection of DTOs, or <c>null</c> if the input is <c>null</c>.</returns>
    protected IEnumerable<TDTO>? ConvertToDTO(IEnumerable<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IEnumerable<TDTO>>(models);
    }

    /// <summary>
    /// Converts a list of entities to a list of DTOs using AutoMapper.
    /// </summary>
    /// <param name="models">The list of entities to convert.</param>
    /// <returns>The converted list of DTOs, or <c>null</c> if the input is <c>null</c>.</returns>
    protected IList<TDTO>? ConvertToDTO(IList<TEntity>? models)
    {
        if (models is null) return null;
        return mapper.Map<IList<TDTO>>(models);
    }

    /// <summary>
    /// Converts an array of entities to an array of DTOs using AutoMapper.
    /// </summary>
    /// <param name="models">The array of entities to convert.</param>
    /// <returns>The converted array of DTOs, or <c>null</c> if the input is <c>null</c>.</returns>
    protected TDTO[]? ConvertToDTO(TEntity[]? models)
    {
        if (models is null) return null;
        return mapper.Map<TDTO[]>(models);
    }

    #endregion

}
