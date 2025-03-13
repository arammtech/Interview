using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.DTOs;

namespace Muqabalati.Web.Controllers
{
    public class InterviewController : Controller
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        // API to return JSON for starting the interview
        [HttpPost]
        public async Task<IActionResult> StartInterview(InterviewRequestDto request)
        {
            // Call the service to generate the JSON session object
            var session = await _interviewService.GenerateInterviewSessionAsync(request);

            // Serialize the session object to JSON and return as IActionResult
            return Json(session);
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
    }
}
