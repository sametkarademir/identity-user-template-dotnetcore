using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.Repositories.Interface;

public interface IUow : IAsyncDisposable
{
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IAppUserRepository AppUserRepository { get; }
    
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
}