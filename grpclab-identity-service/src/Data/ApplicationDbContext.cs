using GRPCLab.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRPCLab.IdentityService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
        private readonly IEnumerable<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = 1,
                DisplayName = "LabA",
                Email = "laba@grpclab.com",
                NormalizedEmail = "LABA@GRPCLAB.COM",
                UserName = "laba@grpclab.com",
                NormalizedUserName = "LABA@GRPCLAB.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            },
            new ApplicationUser
            {
                Id = 2,
                DisplayName = "LabB",
                Email = "labb@grpclab.com",
                NormalizedEmail = "LABB@GRPCLAB.COM",
                UserName = "labb@grpclab.com",
                NormalizedUserName = "LABB@GRPCLAB.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            }
        };
        private readonly string _defaultPassword = "P@s5word";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>().Property(c => c.DisplayName).HasMaxLength(100);
            _users.ToList().ForEach(x =>
            {
                x.PasswordHash = _passwordHasher.HashPassword(x, _defaultPassword);
            });
            builder.Entity<ApplicationUser>().HasData(_users);
            base.OnModelCreating(builder);
        }
    }
}