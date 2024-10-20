namespace Domain.Shared.Abstract;

public class PageQuery
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}