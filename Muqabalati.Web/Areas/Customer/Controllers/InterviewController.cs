using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.DTOs;

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

            /*
             comes:  DTO Interview (contains the parameters)
             
             Take Interview demands from user 

             add interview 

             send the interviewDTO to service the api and receive the InterviewSessionDTO  (questions....)

             Take the InterviewSessionDTO (inside it the questions) 

             display the page of ai 

             after it I will handle it with js and api



            */


            // Call the service to generate the JSON session object
            //var session = await _interviewService.GenerateInterviewSessionAsync(request);

            // Serialize the session object to JSON and return as IActionResult
            //return Json(session);

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
        public async Task<IActionResult> Result(InterviewSessionDto InterviewSessionDTO)
        {
            /*
             after the interview ends and click on (take your result)

             Take the InterviewSessionDTO

             send it to ai 

             get the result from ai

             Go to the result page and display result
             
             */

            return View();
        }


    }
}
