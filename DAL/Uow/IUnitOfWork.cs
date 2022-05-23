using DAL.Entities;
using DAL.Repositories;

namespace DAL.Uow;

public interface IUnitOfWork
{
    public IRepository<User> Users { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }
    public void Save();
}

