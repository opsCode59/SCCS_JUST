using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
    {
    public class Seed
        {
        public static async Task ClearConnection(AppDbContext appDbContext )
            {
            appDbContext.connections.RemoveRange(appDbContext.connections);
            await appDbContext.SaveChangesAsync();
            }
        public static async Task SeedUsers(UserManager<AppUser> userManager ,RoleManager<AppRole> roleManager)
            {
            if (await userManager.Users.AnyAsync()) return;
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
                {
                new AppRole{Name="Admin"},
                new AppRole{Name="Patient"},
                new AppRole{Name="Doctor"},
                new AppRole{Name="Clinic"},
                };
            
            foreach(var role in roles)
                {
                await roleManager.CreateAsync(role);
                }
            

            foreach (var user in users) 
                {
                user.Photos.First().IsApproved = true;
                user.UserName = user.UserName.ToLower();
                user.DateOfBirth = DateTime.SpecifyKind(user.DateOfBirth,DateTimeKind.Utc);
                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
                await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Patient");
                }
            var admin = new AppUser
                {
                UserName = "admin",
                KnownAs= "Admin",
                DateOfBirth = DateTime.UtcNow,
                City ="Irbid",
                Country = "Jordan",
                Gender= "male",
                LastActive = DateTime.UtcNow,
                Created =DateTime.UtcNow,
                };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            
            await userManager.AddToRolesAsync(admin, new[] { "Admin"});
            }
        }
    }
