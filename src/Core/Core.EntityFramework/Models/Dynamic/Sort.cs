using Core.EntityFramework.Models.Enum;

namespace Core.EntityFramework.Models.Dynamic;

public class Sort(string field, DirectionTypes dir)
{
    public Sort() : this(string.Empty, DirectionTypes.Asc)
    {
    }

    public string Field { get; set; } = field;
    public DirectionTypes Dir { get; set; } = dir;
}