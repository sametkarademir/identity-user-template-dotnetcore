namespace Core.EntityFramework.Models.Entity;

public interface IEntityDeleteTimestamps<TId> : IEntityUpdateTimestamps<TId>
{
    DateTime? DeletedDate { get; set; }
    TId? DeleterId { get; set; }
}