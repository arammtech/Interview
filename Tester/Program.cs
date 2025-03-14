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
var gemeni = new GenAIApiService();
var interviewservice = new InterviewService(gemeni);
var test = await interviewservice.GenerateInterviewSessionAsync(new());

Console.WriteLine(test.IntroText);
Console.WriteLine("الاسئلة");
foreach(var q in test.Questions)
{
    Console.WriteLine(q.LinkingPhrase);
    Console.WriteLine(q.OriginalQuestion);
    Console.WriteLine(q.RephrasedQuestion);
    Console.WriteLine(q.Explanation);
    Console.WriteLine("الوقع المتوقع للاجابة" + q.EstimatedTimeMinutes);

}

Console.WriteLine(test.ConclusionText);