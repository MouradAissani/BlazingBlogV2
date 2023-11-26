using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlazingBlogV2;

public static partial class Extensions
{
    public static string GetUserName(this ClaimsPrincipal user) =>
        user.FindFirstValue(AppConstants.ClaimNames.FullName)!;
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public static string ToSlug(this string text)
    {
        text = SlugRegEx()
            .Replace(text.ToLowerInvariant(), "-");
        return text
            .Replace("--", "-")
            .Trim('-');
    }

    [GeneratedRegex(@"[^0-9a-z_]")]
    private static partial Regex SlugRegEx();

    public static string ToDisplay(this DateTime? dateTime) => dateTime?.ToString("MMM dd") ?? string.Empty;
}