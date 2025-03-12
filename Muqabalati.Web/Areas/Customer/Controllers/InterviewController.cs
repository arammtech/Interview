using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.DTOs;
using Muqabalati.Domain.Global;
using Muqabalati.Utilities.Global;

namespace Muqabalati.Web.Controllers
{
    public class InterviewController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly string _apiKey = "AIzaSyCoDx9WJWnE960DVAPV4ddXv3468cyQg84"; // يجب أن يتم تخزين الـ API Key في إعدادات آمنة

        public InterviewController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        // عرض الصفحة الرئيسية
        public IActionResult Index()
        {
            return View();
        }

        // بدء المقابلة
        [HttpPost]
        public async Task<IActionResult> StartInterview(QuestionRequest request)
        {
            // توليد نص المقدمة
            string introText = await _geminiService.GenerateIntroText(_apiKey, request.ApplicantName, request.InterviewerName, request.Topic, request.Level, request.Department);

            // توليد نص الأسئلة
            string questionsResponse = await _geminiService.GenerateQuestionText(_apiKey, 2, request.Topic, request.Level, request.Department, request.Tone, request.Language);

            // توليد نص الخاتمة
            string conclusionText = await _geminiService.GenerateConclusionText(_apiKey);

            // تحليل الأسئلة
            var questionsList = await _geminiService.ParseQuestions(questionsResponse);

            // إنشاء الجلسة
            var session = new InterviewSessionDto
            {
                ApplicantName = request.ApplicantName,
                IntroText = introText,
                Questions = questionsList,
                ConclusionText = conclusionText
            };
           
            // تخزين الجلسة في Session
            HttpContext.Session.Set("InterviewSession", session);
            

            return View("InterviewSession", session);
        }


        // إنهاء الجلسة وعرض النتيجة
        public IActionResult FinishInterview()
        {
            // استرداد الجلسة
            var session = HttpContext.Session.Get<InterviewSessionDto>("InterviewSession");
            if (session == null) return RedirectToAction("Index");
            
            // عرض صفحة التقييم
            return View("Result", session);
        }
    }
}
