using Domain.Shared.Abstract;

namespace Domain.Shared.Dtos;

public class GetPagedListResponseDto<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items { get => _items ??= new List<T>(); set => _items = value; }
}