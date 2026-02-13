using System;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<UserLike> UserLikes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserLike with composite key
        modelBuilder.Entity<UserLike>()
            .HasKey(ul => new { ul.SourceUserId, ul.LikedUserId });

        // Configure relationships
        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.SourceUser)
            .WithMany()
            .HasForeignKey(ul => ul.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.LikedUser)
            .WithMany()
            .HasForeignKey(ul => ul.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Message relationships
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany()
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}