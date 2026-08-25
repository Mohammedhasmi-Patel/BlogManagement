using BlogManagement.Enum;
using BlogManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogManagement.Seeders
{
    public static class UserSeeder
    {
        public static async Task<Dictionary<string, AppUser>> SeedAsync(UserManager<AppUser> userManager)
        {
            var usersMap = new Dictionary<string, AppUser>(StringComparer.OrdinalIgnoreCase);

            var seedUsers = new[]
            {
                new
                {
                    Email = "admin@blogify.com",
                    FirstName = "Alex",
                    LastName = "Administrator",
                    Role = nameof(UserRoleEnum.Admin),
                    Bio = "System Administrator and Community Lead at Blogify. Passionate about software architecture and platform stability.",
                    Avatar = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=400&auto=format&fit=crop&q=80"
                },
                new
                {
                    Email = "sophia.turner@example.com",
                    FirstName = "Sophia",
                    LastName = "Turner",
                    Role = nameof(UserRoleEnum.Author),
                    Bio = "Principal Cloud Architect & Distributed Systems specialist. Writing on ASP.NET Core, Kubernetes, Microservices, and resilient systems.",
                    Avatar = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400&auto=format&fit=crop&q=80"
                },
                new
                {
                    Email = "liam.chen@example.com",
                    FirstName = "Liam",
                    LastName = "Chen",
                    Role = nameof(UserRoleEnum.Author),
                    Bio = "Frontend Architect & Design Systems Lead. Passionate about React 19, TypeScript, micro-interactions, CSS architecture, and web performance.",
                    Avatar = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&auto=format&fit=crop&q=80"
                },
                new
                {
                    Email = "elena.rostova@example.com",
                    FirstName = "Elena",
                    LastName = "Rostova",
                    Role = nameof(UserRoleEnum.Author),
                    Bio = "AI/ML Engineer & Researcher. Exploring LLM fine-tuning, RAG pipelines, neural architectures, and AI-assisted software development.",
                    Avatar = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=400&auto=format&fit=crop&q=80"
                },
                new
                {
                    Email = "david.miller@example.com",
                    FirstName = "David",
                    LastName = "Miller",
                    Role = nameof(UserRoleEnum.User),
                    Bio = "Full-stack developer student, avid tech reader, and open-source enthusiast.",
                    Avatar = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&auto=format&fit=crop&q=80"
                },
                new
                {
                    Email = "marcus.vance@example.com",
                    FirstName = "Marcus",
                    LastName = "Vance",
                    Role = nameof(UserRoleEnum.User),
                    Bio = "Software engineer exploring mobile multiplatform development and cloud infrastructure.",
                    Avatar = "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?w=400&auto=format&fit=crop&q=80"
                }
            };

            const string defaultPassword = "Password123!";

            foreach (var item in seedUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(item.Email);
                if (existingUser == null)
                {
                    var user = new AppUser
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Bio = item.Bio,
                        Avatar = item.Avatar,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    };

                    var result = await userManager.CreateAsync(user, defaultPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, item.Role);
                        usersMap[item.Email] = user;
                    }
                }
                else
                {
                    usersMap[item.Email] = existingUser;
                }
            }

            return usersMap;
        }
    }
}
