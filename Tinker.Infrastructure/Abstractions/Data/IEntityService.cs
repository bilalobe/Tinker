namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface IEntityService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    Task<IEnumerable<TDto>> GetAll();
    Task<TDto?> GetById(int          id);
    Task<TEntity?> GetEntityById(int id);
    Task Add(TDto                    dto);
    Task Update(TDto                 dto);
    Task Delete(int                  id);
}