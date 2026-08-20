using BlogManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Database
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BlogHasCategory> BlogCategories => Set<BlogHasCategory>();
        public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<UserFollow> UserFollows => Set<UserFollow>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // BlogHasCategory composite key and relations
            builder.Entity<BlogHasCategory>(entity =>
            {
                entity.HasKey(bc => new { bc.BlogId, bc.CategoryId });

                entity.HasOne(bc => bc.Blog)
                    .WithMany(b => b.BlogCategories)
                    .HasForeignKey(bc => bc.BlogId);

                entity.HasOne(bc => bc.Category)
                    .WithMany(c => c.BlogCategories)
                    .HasForeignKey(bc => bc.CategoryId);
            });

            // Bookmark composite key and relations
            builder.Entity<Bookmark>(entity =>
            {
                entity.HasKey(bm => new { bm.UserId, bm.BlogId });

                entity.HasOne(bm => bm.User)
                    .WithMany(u => u.Bookmarks)
                    .HasForeignKey(bm => bm.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bm => bm.Blog)
                    .WithMany(b => b.Bookmarks)
                    .HasForeignKey(bm => bm.BlogId);
            });

            // UserFollow composite key and relations
            builder.Entity<UserFollow>(entity =>
            {
                entity.HasKey(uf => new { uf.FollowerId, uf.AuthorId });

                entity.HasOne(uf => uf.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(uf => uf.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uf => uf.Author)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(uf => uf.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Blog relations
            builder.Entity<Blog>(entity =>
            {
                entity.HasOne(b => b.Author)
                    .WithMany(u => u.Blogs)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category relations
            builder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Creator)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment relations
            builder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Blog)
                    .WithMany(b => b.Comments)
                    .HasForeignKey(c => c.BlogId);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Like relations
            builder.Entity<Like>(entity =>
            {
                entity.HasOne(l => l.Blog)
                    .WithMany(b => b.Likes)
                    .HasForeignKey(l => l.BlogId);

                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
