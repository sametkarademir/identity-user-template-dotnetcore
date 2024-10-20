namespace Core.EntityFramework.Models.Entity;

public interface IBasicAggregateRoot<TId>
{
    public TId Id { get; set; }
    public bool IsDeleted { get; set; }
}