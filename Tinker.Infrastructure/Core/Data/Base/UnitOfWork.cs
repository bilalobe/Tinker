// Tinker.Infrastructure/Data/UnitOfWork.cs

using Tinker.Infrastructure.Core.Data.Context;
using Tinker.Infrastructure.Core.Data.Interfaces;

namespace Tinker.Infrastructure.Core.Data.Base;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly IDictionary<Type, object> _repositories = new Dictionary<Type, object>();

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (_repositories.TryGetValue(typeof(T), out var repository))
            return (IRepository<T>)repository;

        var newRepository = new Repository<T>(context);
        _repositories.Add(typeof(T), newRepository);
        return newRepository;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}