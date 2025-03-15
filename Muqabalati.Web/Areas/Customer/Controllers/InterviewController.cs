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
        public async Task<IActionResult> Create()
        {
            /* DTO InterviewRequestDto (contains the parameters)
             Add interview form:
             go to the page that asks the user to choose their demands witha an empty dto to fill
           */

            InterviewRequestDto interviewDto = new(); 

            return View(interviewDto);
        }

        // API to return JSON for starting the interview
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public async Task<IActionResult> StartInterview(InterviewRequestDto request)
        {

            return View();


        }

        // API to return the JSON for the interview session stored in session
        [HttpGet]
        public IActionResult GetInterviewSession()
        {
            string sessionJson = HttpContext.Session.GetString("InterviewSession");

            if (string.IsNullOrEmpty(sessionJson))
            {
                return NotFound(new { message = "Interview session not found." });
            }

            return Ok(sessionJson);
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(List<AnswerModel> answers)
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

            // استدعاء خدمة InterviewService لتوليد التقرير
            var report = await _interviewService.GenerateInterviewReport( answers, questions);

            return View("Result", report);

        }


    }
}
