using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Seeders
{
    public static class CategorySeeder
    {
        public static async Task<Dictionary<string, Category>> SeedAsync(AppDbContext context, Dictionary<string, AppUser> users)
        {
            var categoryMap = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

            if (!users.TryGetValue("sophia.turner@example.com", out var sophia) && users.Count > 0)
            {
                sophia = users.Values.First();
            }

            if (!users.TryGetValue("liam.chen@example.com", out var liam) && users.Count > 0)
            {
                liam = sophia;
            }

            var defaultAuthorId = sophia?.Id ?? Guid.Empty;
            var frontendAuthorId = liam?.Id ?? defaultAuthorId;

            var seedCategories = new[]
            {
                new
                {
                    Name = "Web Development",
                    Slug = "web-development",
                    Description = "Deep dives into frontend frameworks, ASP.NET Core APIs, backend architectures, and modern web standards.",
                    Icon = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = defaultAuthorId
                },
                new
                {
                    Name = "Artificial Intelligence",
                    Slug = "artificial-intelligence",
                    Description = "LLM fine-tuning, neural network architectures, RAG systems, and AI-assisted workflows.",
                    Icon = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = defaultAuthorId
                },
                new
                {
                    Name = "Cloud & DevOps",
                    Slug = "cloud-devops",
                    Description = "Docker containers, Kubernetes orchestration, CI/CD automation pipelines, and scalable cloud infrastructure.",
                    Icon = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = defaultAuthorId
                },
                new
                {
                    Name = "UI/UX Design",
                    Slug = "ui-ux-design",
                    Description = "Design systems, typography, micro-interactions, responsive design patterns, and modern web aesthetics.",
                    Icon = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = frontendAuthorId
                },
                new
                {
                    Name = "Mobile Development",
                    Slug = "mobile-development",
                    Description = "Building cross-platform mobile apps with Flutter, React Native, iOS Swift, and Android Kotlin.",
                    Icon = "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = frontendAuthorId
                },
                new
                {
                    Name = "Career & Culture",
                    Slug = "career-and-culture",
                    Description = "Engineering leadership, remote productivity, career progression, and technical interview strategies.",
                    Icon = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=200&auto=format&fit=crop&q=80",
                    CreatedBy = defaultAuthorId
                }
            };

            foreach (var item in seedCategories)
            {
                var existingCategory = await context.Categories
                    .FirstOrDefaultAsync(c => c.Slug == item.Slug);

                if (existingCategory == null)
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name,
                        Slug = item.Slug,
                        Description = item.Description,
                        Icon = item.Icon,
                        CreatedBy = item.CreatedBy,
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    };

                    await context.Categories.AddAsync(category);
                    categoryMap[category.Slug] = category;
                }
                else
                {
                    categoryMap[existingCategory.Slug] = existingCategory;
                }
            }

            await context.SaveChangesAsync();
            return categoryMap;
        }
    }
}
