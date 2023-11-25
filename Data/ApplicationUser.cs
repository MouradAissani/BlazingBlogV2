using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlazingBlogV2.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required,MaxLength(25)]
        public string Name { get; set; }
    }

}
