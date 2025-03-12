//See https://aka.ms/new-console-Muqabalati for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Muqabalati.Domain.Common.IUnitOfWork;
using Muqabalati.Repository.UnitOfWork;
using Muqabalati.Service.Interfaces;
using Muqabalati.Domain.Entities;
using Muqabalati.Service.Implementations;
using Muqabalati.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Bogus;
using Muqabalati.Service.DTOs.Admin;
using Muqabalati.Service.EmailService;
using Muqabalati.Utilities.Identity;

var services = new ServiceCollection();
var buider = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("ConnectionStrings:DefaultConnection").Value;
services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

//Register dependencies
services.AddSingleton<IUnitOfWork, UnitOfWork>();
services.AddSingleton<ILog, LogService>();

services.Configure<EmailSettings>(buider.GetSection("EmailConfiguration"));
services.AddLogging();

services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.Password.RequiredLength = 4;
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
;
services.AddScoped<EmailService>();
services.AddScoped<IEmailVerificationService, EmailVerificationService>();
services.AddTransient<ITokenService, TokenService>();
services.AddTransient<MailKitEmailSender>();
services.AddTransient<SmtpEmailSender>();
// Assign Defualt Startegy 
services.AddTransient<IEmailSenderStrategy>(provider =>
        provider.GetRequiredService<MailKitEmailSender>()
    );



ServiceProvider serviceProvider;
serviceProvider = services.BuildServiceProvider();

services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromSeconds(59);
});

Console.WriteLine("Email Sender Defualt : Mailkit");

var emailService = serviceProvider.GetRequiredService<EmailService>();
ApplicationUser GenerateFakeUser()
{
    var faker = new Faker();

    return new ApplicationUser
    {
        Id = faker.Random.Int(0, int.MaxValue),
        UserName = faker.Internet.UserName(),
        NormalizedUserName = faker.Internet.UserName().ToUpper(),
        Email = faker.Internet.Email(),
        NormalizedEmail = faker.Internet.Email().ToUpper(),
        EmailConfirmed = faker.Random.Bool(),
        PasswordHash = faker.Internet.Password(),
        SecurityStamp = faker.Random.Guid().ToString(),
        ConcurrencyStamp = faker.Random.Guid().ToString(),
        PhoneNumber = faker.Phone.PhoneNumber(),
        PhoneNumberConfirmed = faker.Random.Bool(),
        TwoFactorEnabled = faker.Random.Bool(),
        LockoutEnd = faker.Random.Bool() ? DateTimeOffset.UtcNow.AddDays(7) : null,
        LockoutEnabled = faker.Random.Bool(),
        AccessFailedCount = faker.Random.Int(0, 5),
        FirstName = faker.Name.FirstName(),
        LastName = faker.Name.LastName()
    };
}

