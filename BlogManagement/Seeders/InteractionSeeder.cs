using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Seeders
{
    public static class InteractionSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            Dictionary<string, AppUser> users,
            List<Blog> blogs)
        {
            if (users.Count == 0 || blogs.Count == 0)
            {
                return;
            }

            var david = users.GetValueOrDefault("david.miller@example.com");
            var marcus = users.GetValueOrDefault("marcus.vance@example.com");
            var liam = users.GetValueOrDefault("liam.chen@example.com");
            var sophia = users.GetValueOrDefault("sophia.turner@example.com");
            var elena = users.GetValueOrDefault("elena.rostova@example.com");

            var publishedBlogs = blogs.Where(b => b.Status == "published").ToList();

            // 1. Comments Seeding
            if (!await context.Comments.AnyAsync() && publishedBlogs.Count > 0)
            {
                var commentsToSeed = new List<Comment>();

                var firstBlog = publishedBlogs[0]; // Microservices
                if (david != null)
                {
                    commentsToSeed.Add(new Comment
                    {
                        Id = Guid.NewGuid(),
                        BlogId = firstBlog.Id,
                        UserId = david.Id,
                        Content = "Exceptional overview! The Polly circuit breaker snippet was exactly what our team needed for our downstream resilience policy.",
                        CreatedAt = DateTime.UtcNow.AddDays(-6)
                    });
                }
                if (marcus != null)
                {
                    commentsToSeed.Add(new Comment
                    {
                        Id = Guid.NewGuid(),
                        BlogId = firstBlog.Id,
                        UserId = marcus.Id,
                        Content = "How do you handle outbox event cleanup in high-throughput PostgreSQL/SQL Server instances?",
                        CreatedAt = DateTime.UtcNow.AddDays(-4)
                    });
                }
                if (sophia != null && marcus != null)
                {
                    commentsToSeed.Add(new Comment
                    {
                        Id = Guid.NewGuid(),
                        BlogId = firstBlog.Id,
                        UserId = sophia.Id,
                        Content = "@marcus Great question! We typically run a background worker that archives processed records to cold storage in batches during off-peak windows.",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    });
                }

                if (publishedBlogs.Count > 1 && david != null)
                {
                    var secondBlog = publishedBlogs[1]; // Modern CSS
                    commentsToSeed.Add(new Comment
                    {
                        Id = Guid.NewGuid(),
                        BlogId = secondBlog.Id,
                        UserId = david.Id,
                        Content = "The clamp() and oklch() color token setup makes responsive themes so much simpler. Loving the glassmorphism aesthetic!",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });
                }

                if (publishedBlogs.Count > 2 && liam != null)
                {
                    var thirdBlog = publishedBlogs[2]; // LLMs
                    commentsToSeed.Add(new Comment
                    {
                        Id = Guid.NewGuid(),
                        BlogId = thirdBlog.Id,
                        UserId = liam.Id,
                        Content = "PagedAttention and vLLM have been game changers for our internal knowledge base query times. Great breakdown Elena!",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });
                }

                if (commentsToSeed.Count > 0)
                {
                    await context.Comments.AddRangeAsync(commentsToSeed);
                }
            }

            // 2. Likes Seeding
            if (!await context.Likes.AnyAsync() && publishedBlogs.Count > 0)
            {
                var likesToSeed = new List<Like>();
                var allUsers = users.Values.ToList();

                foreach (var blog in publishedBlogs)
                {
                    foreach (var user in allUsers.Take(3))
                    {
                        likesToSeed.Add(new Like
                        {
                            Id = Guid.NewGuid(),
                            BlogId = blog.Id,
                            UserId = user.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        });
                    }
                }

                if (likesToSeed.Count > 0)
                {
                    await context.Likes.AddRangeAsync(likesToSeed);
                }
            }

            // 3. Bookmarks Seeding
            if (!await context.Bookmarks.AnyAsync() && publishedBlogs.Count > 0)
            {
                var bookmarksToSeed = new List<Bookmark>();

                if (david != null && publishedBlogs.Count > 0)
                {
                    bookmarksToSeed.Add(new Bookmark
                    {
                        Id = Guid.NewGuid(),
                        UserId = david.Id,
                        BlogId = publishedBlogs[0].Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    });
                }

                if (marcus != null && publishedBlogs.Count > 1)
                {
                    bookmarksToSeed.Add(new Bookmark
                    {
                        Id = Guid.NewGuid(),
                        UserId = marcus.Id,
                        BlogId = publishedBlogs[1].Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    });
                }

                if (bookmarksToSeed.Count > 0)
                {
                    await context.Bookmarks.AddRangeAsync(bookmarksToSeed);
                }
            }

            // 4. User Follows Seeding
            if (!await context.UserFollows.AnyAsync())
            {
                var followsToSeed = new List<UserFollow>();

                if (david != null && sophia != null)
                {
                    followsToSeed.Add(new UserFollow
                    {
                        FollowerId = david.Id,
                        AuthorId = sophia.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    });
                }

                if (david != null && liam != null)
                {
                    followsToSeed.Add(new UserFollow
                    {
                        FollowerId = david.Id,
                        AuthorId = liam.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-12)
                    });
                }

                if (marcus != null && elena != null)
                {
                    followsToSeed.Add(new UserFollow
                    {
                        FollowerId = marcus.Id,
                        AuthorId = elena.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    });
                }

                if (liam != null && sophia != null)
                {
                    followsToSeed.Add(new UserFollow
                    {
                        FollowerId = liam.Id,
                        AuthorId = sophia.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-8)
                    });
                }

                if (followsToSeed.Count > 0)
                {
                    await context.UserFollows.AddRangeAsync(followsToSeed);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
