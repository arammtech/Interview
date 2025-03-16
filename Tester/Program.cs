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
//var test = await interviewservice.GenerateInterviewSessionAsync(new());

//Console.WriteLine(test.IntroText);
//Console.WriteLine("الاسئلة");
//foreach(var q in test.Questions)
//{
//    Console.WriteLine(q.LinkingPhrase);
//    Console.WriteLine(q.OriginalQuestion);
//    Console.WriteLine(q.RephrasedQuestion);
//    Console.WriteLine(q.Explanation);
//    Console.WriteLine("الوقع المتوقع للاجابة" + q.EstimatedTimeMinutes);

//}

//Console.WriteLine(test.ConclusionText);



string[] questions =
{
    "إيه هو \".Net\" وبيستخدم في إيه؟",
    "إيه هو \"PowerShell\" وفايدته إيه في إدارة تكنولوجيا المعلومات؟",
    "\"HTML\" ده إيه ودوره في تطوير الويب؟",
    "إيه هو \"JavaScript\" ودوره في تطوير المواقع؟",
    "إيه فائدة الفارة في الكومبيوتر؟",
    "الإنترنت بيشتغل إزاي؟",
    "إيه هو السيرفر وبيعمل إيه؟",
    "الكيبورد بيستخدم في إيه؟",
    "يعني إيه الذكاء الاصطناعي؟",
    "إيه الفرق بين الموبايل والتابلت؟",
    "الجهاز بتاع WiFi بيعمل إيه؟",
    "الباور بانك بيستخدم في إيه؟",
    "يعني إيه سوفت وير؟",
    "يعني إيه \"SSD\"؟",
    "الرام (RAM) بتعمل إيه؟"
};

List<AnswerModel> answers = new List<AnswerModel>
{
    new AnswerModel { Answer = "إطار عمل تطوير برمجيات تم تطويره بواسطة مايكروسوفت." }, // صحيح
    new AnswerModel { Answer = "برنامج بيقدّم بيئة متكاملة لإدارة وتنفيذ المهام في تكنولوجيا المعلومات." }, // صحيح
    new AnswerModel { Answer = "لغة برمجة لتصميم التطبيقات الذكية." }, // خطأ
    new AnswerModel { Answer = "لغة برمجة بتُستخدم لتفعيل التفاعلية في المواقع." }, // صحيح
    new AnswerModel { Answer = "لتحريك الملفات تلقائيًا على الشاشة." }, // خطأ
    new AnswerModel { Answer = "باستخدام كابلات بحرية وأقمار صناعية لنقل البيانات." }, // صحيح
    new AnswerModel { Answer = "مكان لتخزين الصور بس." }, // خطأ
    new AnswerModel { Answer = "لإدخال البيانات في الكمبيوتر." }, // صحيح
    new AnswerModel { Answer = "إنه القدرة على حل المعادلات الرياضية فقط." }, // خطأ
    new AnswerModel { Answer = "التابلت بيجي بشاشة أكبر من الموبايل عادةً." }, // صحيح
    new AnswerModel { Answer = "بيشغل برامج الكومبيوتر." }, // خطأ
    new AnswerModel { Answer = "لشحن الأجهزة المحمولة." }, // صحيح
    new AnswerModel { Answer = "البرامج اللي بتشتغل على أجهزة الكمبيوتر." }, // صحيح
    new AnswerModel { Answer = "هو نوع من أنواع الأقراص المدمجة القديمة." }, // خطأ
    new AnswerModel { Answer = "هي الذاكرة اللي بتساعد الجهاز يعالج البيانات بسرعة." }  // صحيح
};

// استدعاء الخدمة
var report = await interviewservice.GenerateInterviewReport(answers, questions);

// عرض النتائج
Console.WriteLine($"TotalQuestions: {report.TotalQuestions}");
Console.WriteLine($"Correct Answers: {report.CorrectAnswers}");
Console.WriteLine($"Fail Answers: {report.FailAnswers}");
Console.WriteLine($"GPA: {report.GPA}");
Console.WriteLine($"IsPassed: {report.IsPassed}");

Console.WriteLine("Recommendations:");
foreach (var recommendation in report.Recommendations)
{
    Console.WriteLine($"- {recommendation.Recommendation} (Source: {recommendation.Source})");
}
