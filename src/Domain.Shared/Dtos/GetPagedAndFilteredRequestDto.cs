using Core.EntityFramework.Models.Dynamic;
using Domain.Shared.Abstract;

namespace Domain.Shared.Dtos;

public class GetPagedAndFilteredRequestDto
{
    public PageQuery PageRequest { get; set; } = null!;
    public DynamicQuery? DynamicQuery { get; set; }
}