using BlazingBlogV2.Data.Entities;
using BlazingBlogV2.Models;

namespace BlazingBlogV2.Services;

public interface IBlogPostAdminService
{
    Task<PagedResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);
    Task<BlogPost> GetBlogPostByIdAsync(int id);
    Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId);
}