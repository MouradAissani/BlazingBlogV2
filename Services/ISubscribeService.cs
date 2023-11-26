using BlazingBlogV2.Data.Entities;

namespace BlazingBlogV2.Services;

public interface ISubscribeService
{
    Task<string?> SubscribeAsync(Subscriber subscriber);
}