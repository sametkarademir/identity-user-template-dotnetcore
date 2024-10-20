using Core.EntityFramework.Repositories;
using Domain.Models;

namespace Persistence.Repositories.Interface;

public interface IRefreshTokenRepository : IAsyncRepository<RefreshToken, Guid>
{
    
}