namespace Core.EntityFramework.Models.Entity;

public interface IEntityUpdateTimestamps<TId> : IEntityCreationTimestamps<TId>
{
    DateTime? UpdatedDate { get; set; }
    TId? UpdaterId { get; set; }
}