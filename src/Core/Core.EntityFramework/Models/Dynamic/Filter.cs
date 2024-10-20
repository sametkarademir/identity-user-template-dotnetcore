using Core.EntityFramework.Models.Enum;

namespace Core.EntityFramework.Models.Dynamic;

public class Filter(string field, OperatorTypes @operator)
{
    public Filter() : this(string.Empty, OperatorTypes.Eq)
    {
    }

    public string Field { get; set; } = field;
    public string? Value { get; set; }
    public OperatorTypes Operator { get; set; } = @operator;
    public LogicTypes? Logic { get; set; }

    public IEnumerable<Filter>? Filters { get; set; }
}