﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Interview.Domain.Entities;
using Interview.Domain.Identity;

namespace Interview.Repository.EntityFrameworkCore.Context
{
    public class AppDbContext :  IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext() : base() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
