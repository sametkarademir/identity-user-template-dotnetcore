using System.Linq.Expressions;
using Core.EntityFramework.Models.Dynamic;
using Core.EntityFramework.Models.Paging;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Persistence.Repositories.Interface;

public interface IAppUserRepository
{
    Task<Paginate<AppUser>> GetListAsync(Expression<Func<AppUser, bool>>? predicate = null,
        Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>? orderBy = null,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, int index = 0, int size = 10,
        bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<Paginate<AppUser>> GetListByDynamicAsync(DynamicQuery dynamic,
        Expression<Func<AppUser, bool>>? predicate = null,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, int index = 0, int size = 10,
        bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<List<AppUserRole>> GetListByUserIdAsync(string userId);
    Task<List<AppUserRole>> GetListByRoleIdAsync(string roleId);
}