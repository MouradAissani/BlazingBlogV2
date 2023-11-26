using BlazingBlogV2.Data.Entities;

namespace BlazingBlogV2.Services;

public interface ICategoryService
{
    Task<Category[]> GetCategoriesAsync();
    Task<Category> SaveCategoryAsync(Category category);
    Task<Category?> GetCategoryBySlug(string categorySlug);
}