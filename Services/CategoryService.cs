using BlazingBlogV2.Data;
using BlazingBlogV2.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazingBlogV2.Services;

public class CategoryService : ICategoryService
{
    readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await query.Invoke(context);
    }
    public async Task<Category[]> GetCategoriesAsync()
    {
        return await ExecuteOnContext(async context =>
         {
             var categories = await context.Categories.AsNoTracking().ToArrayAsync();
             return categories;
         });
    }

    public async Task<Category> SaveCategoryAsync(Category category)
    {
        return await ExecuteOnContext(async context =>
        {
            if (category.Id == 0)
            {
                if (await context.Categories.AsNoTracking().AnyAsync(c => c.Name == category.Name))
                {
                    throw new InvalidOperationException($"Category with the name{category.Name} already exist");
                }
                category.Slug = category.Name.ToSlug();
                await context.Categories.AddAsync(category);

            }
            else
            {
                if (await context.Categories.AsNoTracking().AnyAsync(c => c.Name == category.Name
                                                                          && c.Id != category.Id))
                {
                    throw new InvalidOperationException($"Category with the name{category.Name} already exist");
                }

                var dbCategory = await context.Categories.FindAsync(category.Id);
                dbCategory!.Name = category.Name;
                dbCategory!.ShowOnNavbar = category.ShowOnNavbar;

            }
            await context.SaveChangesAsync();
            return category;
        });
    }

    public async Task<Category?> GetCategoryBySlug(string categorySlug)
        => await ExecuteOnContext(async context =>
        await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == categorySlug));

}