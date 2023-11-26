using BlazingBlogV2.Data;
using BlazingBlogV2.Data.Entities;
using BlazingBlogV2.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazingBlogV2.Services;

public class BlogPostService : IBlogPostService
{
    readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    async Task<TResult> QueryOnContexAsync<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await query(context);
    }

    public BlogPostService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0)
    {
        return await QueryOnContexAsync(async context =>
        {
            var query = context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.IsPublished);

            if (categoryId > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            var records = await query.Where(b => b.IsFeatured).OrderBy(_ => Guid.NewGuid()).Take(count).ToArrayAsync();
            if (count > records.Length)
            {
                var additionalRecords = await query
                    .Where(b => !b.IsFeatured)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(count - records.Length).ToArrayAsync();

                records = [.. records, .. additionalRecords];
            }

            return records;
        });
    }

    public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0)
    {
        return await QueryOnContexAsync(async context =>
        {
            var query = context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.IsPublished);

            if (categoryId > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            return await query.OrderByDescending(b => b.ViewCount).Take(count).ToArrayAsync();
        });
    }

    public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0)
    => await GetPostsAsync(0, count, categoryId);

    public async Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId = 0)
        => await GetPostsAsync(pageIndex * pageSize, pageSize, categoryId);



    public async Task<DetailPageModel> GetBlogPostBySlugAsync(string slug)
    {

        return await QueryOnContexAsync(async context =>
        {
            var blogPost = await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

            if (blogPost is null) return DetailPageModel.Empty();

            var relatedPosts = await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.CategoryId == blogPost.CategoryId && b.IsPublished)
                .Take(4)
                .ToArrayAsync();


            return new DetailPageModel(blogPost, relatedPosts);
        });
    }
    async Task<BlogPost[]> GetPostsAsync(int skip, int take, int categoryId)
    {
        return await QueryOnContexAsync(async context =>
        {
            var query = context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.IsPublished);

            if (categoryId > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            return await query
                .OrderByDescending(b => b.PublishedAt)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();
        });
    }
}