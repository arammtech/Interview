using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.DTOs;
using Newtonsoft.Json;

namespace Muqabalati.Web.Controllers
{
    [Area("Customer")]
    public class InterviewController : Controller
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet]
        public IActionResult InterviewGenerator()
        {
            // new dto and send
            return View();
        }
       
        [HttpGet]
        public async Task<IActionResult> StartInterview()
        {
            InterviewRequestDto interviewDto = new();

            return View(interviewDto);
        }


        // // // // // move this to the api controller
        [HttpPost]
        public async Task<IActionResult> Result([FromBody]  List<AnswerModel> answers)
        {
            //List<AnswerModel> answers1 = new List<AnswerModel>
            //{
            //    new AnswerModel { Answer = "A software development framework developed by Microsoft" },
            //    new AnswerModel { Answer = "An integrated environment for managing and automating tasks in IT" },
            //    new AnswerModel { Answer = "It is a markup language used for creating web pages", },
            //    new()
            //};

            //string[] questions1 =
            //{
            //      "What is .Net, and what is its purpose?",
            //        "What is PowerShell, and why is it useful in IT management?",
            //        "What is HTML, and what is its role in web development?",
            //        "What is JS, and what is its role in web development?"

            //};

            try
            {
                string interviewSessionJson = HttpContext.Session.GetString("InterviewSession");

                if (string.IsNullOrEmpty(interviewSessionJson))
                {
                    return BadRequest("Interview session not found.");
                }

                var session = JsonConvert.DeserializeObject<InterviewSessionDto>(interviewSessionJson);

                if (session == null || session.Questions == null)
                {
                    return BadRequest("Invalid interview session data.");
                }

                var questions = session.Questions.Select(q => q.OriginalQuestion).ToArray();

                var report = await _interviewService.GenerateInterviewReport(answers, questions);

                TempData["InterviewReport"] = JsonConvert.SerializeObject(report);
                
                return Ok(new { success = true, data = report });
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult InterviewResult()
        {
            if (TempData["InterviewReport"] is string ratingJson)
            {
                var report = JsonConvert.DeserializeObject<InterviewReportDto>(ratingJson);
            
                return View("InterviewResult", report); 
            }
            else
            {
                return View("InterviewIsOver", "المقابلة لقد انتهت");
            }
        }

        [HttpGet]
        public IActionResult InterviewIsOver(string reason)
        {
            return View(reason);
        }
        
    }
}
