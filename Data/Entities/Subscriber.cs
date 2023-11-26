using System.ComponentModel.DataAnnotations;

namespace BlazingBlogV2.Data.Entities;

public class Subscriber
{
    public long Id { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime SubscribedOn { get; set; }
}