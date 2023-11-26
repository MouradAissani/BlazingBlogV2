namespace BlazingBlogV2.Models;

public record PagedResult<TResult>(TResult[] Records, int TotalCount);