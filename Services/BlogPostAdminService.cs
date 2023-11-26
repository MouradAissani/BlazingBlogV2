using BlazingBlogV2.Data;
using BlazingBlogV2.Data.Entities;
using BlazingBlogV2.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazingBlogV2.Services;

public class BlogPostAdminService : IBlogPostAdminService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public BlogPostAdminService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public async Task<PagedResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
    {
        return await ExecuteOnContext(async context =>
        {
            var query = context.BlogPosts.AsNoTracking();
            var count = await query.CountAsync();
            var records = await query
                .Include(c => c.Category)
                .OrderByDescending(b => b.Id)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync();
            return new PagedResult<BlogPost>(records, count);
        });
    }

    public async Task<BlogPost> GetBlogPostByIdAsync(int id) =>
        await ExecuteOnContext(async context =>
        await context.BlogPosts.AsNoTracking().Include(c => c.Category).FirstOrDefaultAsync(b => b.Id == id));



    public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId)
    {
        return await ExecuteOnContext(async context =>
        {
            if (blogPost.Id == 0)
            {
                var duplicatedTitle = await context.BlogPosts.AsNoTracking().AnyAsync(b => b.Title == blogPost.Title);
                if (duplicatedTitle)
                {
                    throw new InvalidOperationException($"Blog post xith this same title already exists");
                }

                blogPost.Slug = await GenerateSlugAsync(blogPost);
                blogPost.CreatedAt = DateTime.UtcNow;
                blogPost.UserId = userId;
                if (blogPost.IsPublished)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }

                await context.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                var duplicatedTitle = await context.BlogPosts.AsNoTracking().AnyAsync(b => b.Title == blogPost.Title && b.Id != blogPost.Id);
                if (duplicatedTitle)
                {
                    throw new InvalidOperationException($"Blog post xith this same title already exists");
                }

                var dbBlog = await context.BlogPosts.FindAsync(blogPost.Id);
                dbBlog.Title = blogPost.Title;
                dbBlog.Image = blogPost.Image;
                dbBlog.Introduction = blogPost.Introduction;
                dbBlog.Content = blogPost.Content;
                dbBlog.CategoryId = blogPost.CategoryId;
                dbBlog.IsFeatured = blogPost.IsFeatured;
                dbBlog.IsPublished = blogPost.IsPublished;
                if (blogPost.IsPublished)
                {
                    if (!dbBlog.IsPublished)
                    {
                        blogPost.PublishedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    blogPost.PublishedAt = null;
                }
            }

            await context.SaveChangesAsync();
            return blogPost;
        });
    }
    async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await query.Invoke(context);
    }
    async Task<string> GenerateSlugAsync(BlogPost blogPost)
    {
        return await ExecuteOnContext(async context =>
        {
            string originalSlug = blogPost.Title.ToSlug();
            string slug = originalSlug;
            int count = 1;
            while (await context.BlogPosts.AsNoTracking().AnyAsync(b => b.Slug == slug))
            {
                slug = $"{originalSlug}-{count++}";
            }

            return slug;
        });
    }
}