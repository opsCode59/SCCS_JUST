using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class AppDbContext : 
    IdentityDbContext<AppUser,AppRole,int,
        IdentityUserClaim<int>,
        AppUserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>

    {
    public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
    {
        
    }
    protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
        base.OnModelCreating(modelBuilder);
      
        modelBuilder.Entity<Schedule>()
        .HasIndex(s => s.userName)
         .IsUnique();

        modelBuilder.Entity<Feedback>()
            .HasOne(user=>user.AppUser)
            .WithMany(x=>x.feedbacks)
            .HasForeignKey(feedbacks => feedbacks.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.appUserRoles)
            .WithOne(u => u.AppUser)
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        modelBuilder.Entity<AppRole>()
          .HasMany(u => u.appUserRoles)
          .WithOne(u => u.Role)
          .HasForeignKey(u => u.RoleId)
          .IsRequired();


        modelBuilder.Entity<UserLike>()
            .HasKey(k => new {k.SourceUserId,k.TargetUserId});
        
        modelBuilder.Entity<UserLike>()
            .HasOne(e => e.SourceUser)
            .WithOne(e => e.LikedUser)
        .HasForeignKey<UserLike>(ul => ul.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
            .HasOne(e => e.TargetUser)
            .WithOne(e => e.LikedByUser)
        .HasForeignKey<UserLike>(ul => ul.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(e => e.Recipient)
            .WithMany(e => e.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
       .HasOne(e => e.Sender)
       .WithMany(e => e.MessagesSent)
       .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Photo>().HasQueryFilter(x => x.IsApproved);


        // Configure one-to-many relationship between AppUser and Appointment
        modelBuilder.Entity<AppUser>()
            .HasMany(a => a.Appointments)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.AppUserId)
            .OnDelete(DeleteBehavior.Restrict); // or use DeleteBehavior.Restrict based on your needs

      

        }
    public DbSet<AppUser> appUsers { get; set; }
    public DbSet<Schedule> schedules { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<UserLike> userLikes { get; set; }
    public DbSet<Message> messages { get; set; }
    public DbSet<Group> groups { get; set; }
    public DbSet<Connection> connections { get; set; }
    public DbSet<Feedback> feedbacks { get; set; }
    public DbSet<Appointment> appointments { get; set; }
    public DbSet<Post> posts { get; set; }
    }
