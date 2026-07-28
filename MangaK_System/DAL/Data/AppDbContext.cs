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

            builder.HasOne(x => x.Supervisor).WithMany(x => x.Mangakas).HasForeignKey(x => x.SupervisorId).OnDelete(DeleteBehavior.SetNull);
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
    }
}