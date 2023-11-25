using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BlazingBlogV2.Data.Entities;

public class Category
{
    public short Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(75)]
    public string Slug { get; set; }
    public bool ShowOnNavbar { get; set; }

    public static List<Category> GetSeedCategories()
    => new()
        {
            new () { Name = "C#", Slug = "s-sharp", ShowOnNavbar = true },
            new () { Name = "ASP.NET Cpre", Slug = "asp-net-core", ShowOnNavbar = true },
            new () { Name = "Blazor", Slug = "blazor", ShowOnNavbar = true },
            new () { Name = "SQL Server", Slug = "sql-server", ShowOnNavbar = false },
            new () { Name = "Entity Framework Core", Slug = "ef-core", ShowOnNavbar = true },
            new () { Name = "Angular", Slug = "angular", ShowOnNavbar = false },
            new () { Name = "React", Slug = "react", ShowOnNavbar = false },
            new () { Name = "Vue", Slug = "vue", ShowOnNavbar = true },
            new () { Name = "JavaScript", Slug = "javascript", ShowOnNavbar = false },
            new () { Name = "HTML", Slug = "html", ShowOnNavbar = false },
            new () { Name = "CSS", Slug = "css", ShowOnNavbar = false },
            new () { Name = "Bootstrap", Slug = "bootstrap", ShowOnNavbar = false },
            new () { Name = "MVC", Slug = "mvc", ShowOnNavbar = true }
        };

}