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
        private const string SessionKey = "InterviewSession";


        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet]
        public IActionResult InterviewGenerator()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            InterviewRequestDto request = new();
            request.Topics = ["Machine Learning Engineer", "Software Developer", ".Net Developer", "Full-Stack Developer", "UI/UX Designer", "Graphic Designer", "Full-Stack Developer"];

            return View(request);
        }


        [HttpPost]
        public async Task<IActionResult> InterviewGenerator(InterviewRequestDto request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var interviewSessionDto = await _interviewService.GenerateInterviewSessionAsync(request);
                    
                    // Store the session JSON in HttpContext.Session
                    HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(interviewSessionDto));

                    HttpContext.Session.SetString("tone", request.Tone);

                    return View("StartInterview", interviewSessionDto.Tone);

                }
                catch (System.Exception ex)
                {
                    return View("Error");
                }
            }

            request.Topics = new List<string> { "Machine Learning Engineer", "Software Developer", ".Net Developer", "Full-Stack Developer", "UI/UX Designer", "Graphic Designer", "Full-Stack Developer" };
            return View(request);
            
        }


        [HttpGet]
        public async Task<IActionResult> StartInterview()
        {
            return View();
        }


      
        [HttpGet]
        public async Task<IActionResult> InterviewResult()
        {

            if (TempData["InterviewReport"] is string ratingJson)
            {
                var report = JsonConvert.DeserializeObject<InterviewReportDto>(ratingJson);

                return View("InterviewResult", report);
            }
            else
            {
                return RedirectToAction("InterviewGenerator");
                //return View("InterviewIsOver", "المقابلة لقد انتهت");
            }
        }

        [HttpGet]
        public IActionResult InterviewIsOver(string reason)
        {
            return View(reason);
        }
        
    }
}
