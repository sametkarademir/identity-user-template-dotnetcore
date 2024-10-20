using Core.EntityFramework.Repositories;
using Domain.Models;
using Persistence.Contexts;
using Persistence.Repositories.Interface;

namespace Persistence.Repositories.Concrete;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, Guid, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context) : base(context)
    {
    }
}