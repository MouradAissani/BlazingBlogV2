using BlazingBlogV2.Data;
using BlazingBlogV2.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazingBlogV2.Services;

public class SubscribeService : ISubscribeService
{
    readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public SubscribeService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<string?> SubscribeAsync(Subscriber subscriber)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var isAlreadySubscribed = await dbContext.Subscribers
            .AsNoTracking()
            .AnyAsync(s => s.Email == subscriber.Email);
        if (isAlreadySubscribed)
        {
            return "you are already subscriber";
        }
        subscriber.SubscribedOn = DateTime.UtcNow;
        await dbContext.Subscribers.AddAsync(subscriber);
        await dbContext.SaveChangesAsync();
        return null;
    }
}