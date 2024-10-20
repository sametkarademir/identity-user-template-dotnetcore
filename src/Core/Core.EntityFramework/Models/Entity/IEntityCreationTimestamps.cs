namespace Core.EntityFramework.Models.Entity;

public interface IEntityCreationTimestamps<TId>
{
    DateTime CreatedDate { get; set; }
    TId? CreatorId { get; set; }
}