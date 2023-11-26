using BlazingBlogV2.Data;
using BlazingBlogV2.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazingBlogV2.Services;

internal static class AdminAccount
{
    public const string Name = "Mourad";
    public const string Email = "aissmour@hotmail.fr";
    public const string Role = "Admin";
    public const string Password = "P@ssw0rd";
}
public class SeedService : ISeedService
{
    readonly ApplicationDbContext _dbContext;
    readonly IUserStore<ApplicationUser> _userStore;
    readonly UserManager<ApplicationUser> _userManager;
    readonly RoleManager<IdentityRole> _roleManager;

    public SeedService(ApplicationDbContext dbContext,
        IUserStore<ApplicationUser> userStore,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userStore = userStore;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedDataAsync()
    {
        await MigrateDatabaseAsync();
        // seed admin role
        if (await _roleManager.FindByNameAsync(AdminAccount.Role) is null)
        {
            var adminRole = new IdentityRole(AdminAccount.Role);
            var result = await _roleManager.CreateAsync(adminRole);
            if (!result.Succeeded)
            {
                var errorsString = string.Join(Environment.NewLine,
                    result.Errors.Select(e => e.Description));
                throw new Exception($"Error while creating the admin role " +
                                    $"{Environment.NewLine}" +
                                    $"{errorsString}");
            }
        }
        // seed admin user
        var adminUser = await _userManager.FindByEmailAsync(AdminAccount.Email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser();
            adminUser.Name = AdminAccount.Name;
            adminUser.Email = AdminAccount.Email;
            await _userStore.SetUserNameAsync(adminUser,
                adminUser.Email,
                cancellationToken: CancellationToken.None);
            var result = await _userManager.CreateAsync(adminUser, AdminAccount.Password);
            if (!result.Succeeded)
            {
                var errorsString = string.Join(Environment.NewLine,
                    result.Errors.Select(e => e.Description));
                throw new Exception($"Error while creating the admin user " +
                                    $"{Environment.NewLine}" +
                                    $"{errorsString}");
            }

        }
        // seed categories
        if (!await _dbContext.Categories.AsNoTracking().AnyAsync())
        {
            await _dbContext.Categories.AddRangeAsync(Category.GetSeedCategories());
            await _dbContext.SaveChangesAsync();
        }
    }

    async Task MigrateDatabaseAsync()
    {
#if DEBUG
        if ((await _dbContext.Database.GetAppliedMigrationsAsync()).Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
#endif
    }
}