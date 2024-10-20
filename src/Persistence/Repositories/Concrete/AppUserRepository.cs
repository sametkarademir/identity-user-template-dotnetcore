using System.Linq.Expressions;
using Core.EntityFramework.Extensions;
using Core.EntityFramework.Models.Dynamic;
using Core.EntityFramework.Models.Paging;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Contexts;
using Persistence.Repositories.Interface;

namespace Persistence.Repositories.Concrete;

public class AppUserRepository(BaseDbContext context) : IAppUserRepository
{
    public IQueryable<AppUser> Query() => context.Set<AppUser>();
    
    public async Task<Paginate<AppUser>> GetListAsync(Expression<Func<AppUser, bool>>? predicate = null, Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>? orderBy = null, Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<AppUser> queryable = Query(); 
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }
    
    public async Task<Paginate<AppUser>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<AppUser, bool>>? predicate = null, Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<AppUser> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<List<AppUserRole>> GetListByUserIdAsync(string userId)
    {
        IQueryable<AppUserRole> queryable = context.Set<AppUserRole>();
        queryable = queryable.Where(item => item.UserId == userId);
        queryable = queryable.Include(item => item.AppRole);
        return await queryable.ToListAsync();
    }
    
    public async Task<List<AppUserRole>> GetListByRoleIdAsync(string roleId)
    {
        IQueryable<AppUserRole> queryable = context.Set<AppUserRole>();
        queryable = queryable.Where(item => item.RoleId == roleId);
        queryable = queryable.Include(item => item.AppUser);
        return await queryable.ToListAsync();
    }
}