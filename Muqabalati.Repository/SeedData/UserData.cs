using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Muqabalati.Domain.Entities;
using Muqabalati.Domain.Identity;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Muqabalati.Utilities.Identity;

namespace Muqabalati.Utilities.SeedData
{

    public class UserData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppDbContext _context;

        public UserData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

       


        public static List<(ApplicationUser, string)> LoadUsers()
        {
            return new List<(ApplicationUser, string)>
            {
                (new ApplicationUser { UserName = "admin1", FirstName = "Admin", LastName = "User", Email = "admin.user1@example.com", PhoneNumber = "1234567890" }, AppUserRoles.RoleAdmin),
                (new ApplicationUser { UserName = "user2", FirstName = "Jane", LastName = "Doe", Email = "jane.doe2@example.com", PhoneNumber = "1234567891" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user3", FirstName = "Alice", LastName = "Smith", Email = "alice.smith3@example.com", PhoneNumber = "1234567892" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user4", FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson4@example.com", PhoneNumber = "1234567893" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user5", FirstName = "Charlie", LastName = "Brown", Email = "charlie.brown5@example.com", PhoneNumber = "1234567894" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user6", FirstName = "David", LastName = "Williams", Email = "david.williams6@example.com", PhoneNumber = "1234567895" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user7", FirstName = "Emma", LastName = "Jones", Email = "emma.jones7@example.com", PhoneNumber = "1234567896" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user8", FirstName = "Frank", LastName = "Miller", Email = "frank.miller8@example.com", PhoneNumber = "1234567897" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user9", FirstName = "Grace", LastName = "Davis", Email = "grace.davis9@example.com", PhoneNumber = "1234567898" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user10", FirstName = "Henry", LastName = "Garcia", Email = "henry.garcia10@example.com", PhoneNumber = "1234567899" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user11", FirstName = "Isabel", LastName = "Martinez", Email = "isabel.martinez11@example.com", PhoneNumber = "1234567800" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user12", FirstName = "Jack", LastName = "Rodriguez", Email = "jack.rodriguez12@example.com", PhoneNumber = "1234567801" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user13", FirstName = "Karen", LastName = "Martinez", Email = "karen.martinez13@example.com", PhoneNumber = "1234567802" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user14", FirstName = "Leo", LastName = "Martinez", Email = "leo.martinez14@example.com", PhoneNumber = "1234567803" }, AppUserRoles.RoleCustomer),
                (new ApplicationUser { UserName = "user15", FirstName = "Mia", LastName = "Martinez", Email = "mia.martinez15@example.com", PhoneNumber = "1234567804" }, AppUserRoles.RoleCustomer)
            };
        }
    }
}
