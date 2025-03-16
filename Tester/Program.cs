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
using Muqabalati.Service.DTOs;


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
foreach (var q in test.Questions)
{
    Console.WriteLine(q.LinkingPhrase);
    Console.WriteLine(q.OriginalQuestion);
    Console.WriteLine(q.RephrasedQuestion);
    Console.WriteLine(q.Explanation);
    Console.WriteLine("الوقع المتوقع للاجابة" + q.EstimatedTimeMinutes);

}

Console.WriteLine(test.ConclusionText);



List<AnswerModel> answers = new List<AnswerModel>
{
    new AnswerModel { Answer = "A software development framework developed by Microsoft" },
    new AnswerModel { Answer = "An integrated environment for managing and automating tasks in IT" },
    new AnswerModel { Answer = "It is a markup language used for creating web pages", },
    new()
};

string[] questions =
{
    "What is .Net, and what is its purpose?",
    "What is PowerShell, and why is it useful in IT management?",
    "What is HTML, and what is its role in web development?",
    "What is JS, and what is its role in web development?"

};

//// استدعاء الخدمة
//var report = await interviewservice.GenerateInterviewReport(answers, questions);

// عرض النتائج
Console.WriteLine($"Correct Answers: {report.CorrectAnswers}");
Console.WriteLine($"Fail Answers: {report.FailAnswers}");
Console.WriteLine($"GPA: {report.GPA}");
Console.WriteLine("Recommendations:");
foreach (var recommendation in report.Recommendations)
{
    Console.WriteLine($"- {recommendation.Recommendation} (Source: {recommendation.Source})");
}
