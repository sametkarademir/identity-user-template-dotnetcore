using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Contexts;
using Persistence.Repositories.Concrete;
using Persistence.Repositories.Interface;

namespace Money.Map.Persistence.Repositories.Concrete;

public class Uow(BaseDbContext context) : IUow
{
    public IRefreshTokenRepository RefreshTokenRepository => new RefreshTokenRepository(context);
    public IAppUserRepository AppUserRepository => new AppUserRepository(context);

    public IDbContextTransaction BeginTransaction() { return context.Database.BeginTransaction(); }
    public async Task<IDbContextTransaction> BeginTransactionAsync() { return await context.Database.BeginTransactionAsync(); }
    public async ValueTask DisposeAsync() { await context.DisposeAsync(); }
}