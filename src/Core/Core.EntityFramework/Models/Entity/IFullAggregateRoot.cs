namespace Core.EntityFramework.Models.Entity;

public interface IFullAggregateRoot<TId> : IBasicAggregateRoot<TId>, IEntityDeleteTimestamps<TId>
{
    public string ConcurrencyStamp { get; set; }
}