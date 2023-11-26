using BlazingBlogV2.Data.Entities;
using BlazingBlogV2.Models;

namespace BlazingBlogV2.Services;

public interface IBlogPostService
{
    Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0);
    Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0);
    Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0);
    Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId = 0);
    Task<DetailPageModel> GetBlogPostBySlugAsync(string slug);
}