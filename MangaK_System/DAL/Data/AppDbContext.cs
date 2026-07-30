using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mangak_System._1_DAL.Entity;
using MangaK_System.DAL.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mangak_System._1_DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Series> Series => Set<Series>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<CategorySeries> CategorySeries => Set<CategorySeries>();

    public DbSet<PublishingSchedule> PublishingSchedules => Set<PublishingSchedule>();

    public DbSet<Feedback> Feedbacks => Set<Feedback>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(128);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
            builder.Property(x => x.AuthorName).HasMaxLength(150);
            builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(50).HasDefaultValue(UserRole.Reader);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).HasDefaultValue(UserStatus.Active);

            builder.HasMany(x => x.CreatedSeries).WithOne(x => x.CreatedBy).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.ApprovedSeries).WithOne(x => x.ApprovedBy).HasForeignKey(x => x.ApprovedById).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.ReviewedSeries).WithOne(x => x.ReviewedBy).HasForeignKey(x => x.ReviewedById).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.PublishingSchedules).WithOne(x => x.DecidedBy).HasForeignKey(x => x.DecidedById).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Feedbacks).WithOne(x => x.Sender).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Supervisor).WithMany(x => x.Mangakas).HasForeignKey(x => x.SupervisorId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Series>(builder =>
        {
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(3000);
            builder.Property(x => x.CoverFile).HasMaxLength(500);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

            builder.HasOne(x => x.PublishingSchedule).WithOne(x => x.Series).HasForeignKey<PublishingSchedule>(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Feedbacks).WithOne(x => x.Series).HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PublishingSchedule>(builder =>
        {
            builder.Property(x => x.PublishDate).IsRequired();
            builder.Property(x => x.PublishPeriod).HasMaxLength(50);

            builder.HasIndex(x => x.SeriesId).IsUnique();
        });

        modelBuilder.Entity<Feedback>(builder =>
        {
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.IsRead).HasDefaultValue(false);

            builder.HasIndex(x => x.SenderId);
            builder.HasIndex(x => x.SeriesId);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<CategorySeries>(builder =>
        {
            builder.HasKey(x => new { x.CategoryId, x.SeriesId });

            builder.HasOne(x => x.Category).WithMany(x => x.CategorySeries).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Series).WithMany(x => x.CategorySeries).HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
        });

        // =========================
        // SEED DATA
        // =========================
        var now = DateTimeOffset.Parse("2024-01-01T00:00:00Z");
        var togoroId = Guid.Parse("e24a52c3-96cb-4034-88db-4e1bba697841");
        var mikaId = Guid.Parse("f39b61d2-74ba-42a1-9a7c-3b0acc786520");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
                FirstName = "Admin",
                LastName = "System",
                Email = "admin.mangak@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0902234506",
                Bio = "Handles account management.",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
                FirstName = "Mangaka",
                LastName = "System",
                Email = "mangaksystem.admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0865326785",
                Bio = "Handles account management.",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
                FirstName = "Editorial",
                LastName = "Board",
                Email = "board.mangak@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0902234507",
                Bio = "Oversees final approval and publication planning.",
                Role = UserRole.Editorial,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = togoroId,
                FirstName = "Togoro",
                LastName = "Mori",
                Email = "tranmaiconghung@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234567",
                Bio = "Official Tantou account.",
                Role = UserRole.Tantou,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = mikaId,
                FirstName = "Mika",
                LastName = "Ayashi",
                Email = "michael.anderson@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0975633219",
                Bio = "Fantasy editor and content quality.",
                Role = UserRole.Tantou,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000000"),
                FirstName = "Akira",
                LastName = "Kobayashi",
                Email = "nhathanhthinguyen@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234565",
                AuthorName = "Akira Koba",
                Bio = "A versatile manga creator known for sci-fi adventures and immersive world-building.",
                Role = UserRole.Mangaka,
                SupervisorId = mikaId,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000000"),
                FirstName = "Haruto",
                LastName = "Sato",
                Email = "haruto.sato@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234561",
                AuthorName = "Haru Sato",
                Bio = "A manga artist specializing in action and adventure stories with dynamic illustrations.",
                Role = UserRole.Mangaka,
                SupervisorId = mikaId,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000000"),
                FirstName = "Ren",
                LastName = "Takahashi",
                Email = "tuanle17082k5@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234562",
                AuthorName = "Ren Taka",
                Bio = "Passionate about psychological mysteries and creating emotionally complex characters.",
                Role = UserRole.Mangaka,
                SupervisorId = mikaId,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000000"),
                FirstName = "Yuki",
                LastName = "Nakamura",
                Email = "yuki.nakamura@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234563",
                AuthorName = "Yuki Naka",
                Bio = "Creates heartwarming slice-of-life and romance manga inspired by everyday Japanese culture.",
                Role = UserRole.Mangaka,
                SupervisorId = togoroId,
                Status = UserStatus.Active,
                CreatedAt = now
            },
            new User
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000000"),
                FirstName = "Kaito",
                LastName = "Fujimoto",
                Email = "kaito.fujimoto@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Phone = "0901234564",
                AuthorName = "Kaito Fuji",
                Bio = "Focuses on dark fantasy and supernatural worlds with highly detailed artwork.",
                Role = UserRole.Mangaka,
                SupervisorId = togoroId,
                Status = UserStatus.Active,
                CreatedAt = now
            }            
        );
    }
}

public class AppDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=MangaK_PRN212;User Id=sa;Password=12345;TrustServerCertificate=True;");
        return new AppDbContext(optionsBuilder.Options);
    }
}