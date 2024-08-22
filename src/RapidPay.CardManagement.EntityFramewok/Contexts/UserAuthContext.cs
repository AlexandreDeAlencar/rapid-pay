using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RapidPay.CardManagement.EntityFramework.Contexts
{
    internal class UserAuthContext(DbContextOptions<UserAuthContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("identity");

            // Optional: Seed initial data if needed
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            );

            var hasher = new PasswordHasher<IdentityUser>();

            var user1 = new IdentityUser
            {
                UserName = "testuser1",
                NormalizedUserName = "TESTUSER1",
                Email = "testuser1@example.com",
                NormalizedEmail = "TESTUSER1@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            };

            user1.PasswordHash = hasher.HashPassword(user1, "Password123!");

            var user2 = new IdentityUser
            {
                UserName = "testuser2",
                NormalizedUserName = "TESTUSER2",
                Email = "testuser2@example.com",
                NormalizedEmail = "TESTUSER2@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            };

            user2.PasswordHash = hasher.HashPassword(user2, "Password123!");

            modelBuilder.Entity<IdentityUser>().HasData(user1, user2);
        }
    }
}
