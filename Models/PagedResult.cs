﻿using BlazingBlogV2.Data.Entities;

namespace BlazingBlogV2.Models;

public record PagedResult<TResult>(TResult[] Records, int TotalCount);

public record DetailPageModel(BlogPost BlogPost, BlogPost[] RelatedPosts)
{
    public static DetailPageModel Empty() => new(default, []);
    public bool IsEmpty => BlogPost is null;
}