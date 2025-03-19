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

                List<QuestionModel> questions2 = new List<QuestionModel>() {
                            new()
                            {
                                LinkingPhrase = "السؤال الأول هو",
                                OriginalQuestion = "إيه هي خبراتك مع .NET Framework والـ .NET Core؟",
                                RephrasedQuestion = "ممكن تحكي لنا إزاي اشتغلت قبل كده على مشاريع باستخدام .NET Framework أو .NET Core؟",
                                Explanation = "السؤال ده عشان نفهم مستوى معرفتك بالتقنيات المختلفة المرتبطة بـ .NET.",
                                EstimatedTimeMinutes = 3
                            },
                            new()
                            {
                                LinkingPhrase = "السؤال الثاني هو",
                                OriginalQuestion = "إزاي بتتعامل مع الأخطاء في الكود؟",
                                RephrasedQuestion = "لو واجهت خطأ في الكود، إيه هي الخطوات اللي بتاخدها عشان تحله؟",
                                Explanation = "ده سؤال بيقيس مهاراتك في التحليل وحل المشاكل.",
                                EstimatedTimeMinutes = 2
                            },
                            new()
                            {
                                LinkingPhrase = "السؤال الثالث هو",
                                OriginalQuestion = "إيه هي تجربتك مع الـ APIs؟",
                                RephrasedQuestion = "اشتغلت قبل كده على إنشاء أو استهلاك APIs؟ لو آه، شاركنا شوية تفاصيل.",
                                Explanation = "السؤال ده بيستكشف خبرتك في التعامل مع الـ APIs والـ Web Services.",
                                EstimatedTimeMinutes = 3
                            },
                            new()
                            {
                                LinkingPhrase = "السؤال الرابع هو",
                                OriginalQuestion = "إيه هي أدوات الـ Version Control اللي اشتغلت بيها؟",
                                RephrasedQuestion = "ممكن تقول لنا إزاي كنت بتستخدم Git أو أي أداة Version Control في شغلك؟",
                                Explanation = "ده سؤال مهم لتقييم قدرتك على العمل في بيئة تعاون مع الفريق.",
                                EstimatedTimeMinutes = 2
                            }
                    };
                        

                List<AnswerModel> answers2 = new List<AnswerModel>
{
    new AnswerModel
    {
        Answer = "عملت على عدة مشاريع باستخدام .NET Framework، زي تطوير تطبيقات ويب باستخدام ASP.NET Web Forms، وكمان اشتغلت على تطبيقات سطح المكتب باستخدام Windows Forms. أما مع .NET Core، استخدمته لتطوير APIs خفيفة وفعّالة باستخدام ASP.NET Core."
    },
    new AnswerModel
    {
        Answer = "لما واجهت أخطاء في الكود، أول خطوة بعملها هي استخدام Debugger في Visual Studio لتحديد المشكلة. لو الموضوع متعلق بالبيانات، بفحص Queries في قاعدة البيانات باستخدام أدوات زي SQL Profiler. ولو كان الأمر معقد، برجع للمستندات الرسمية أو مواقع زي StackOverflow."
    },
    new AnswerModel
    {
        Answer = "اشتغلت على RESTful APIs باستخدام ASP.NET Core لتمكين التواصل بين التطبيقات. مثال عملي كان إنشاء API لإدارة بيانات المستخدمين في مشروع e-commerce، وكمان استخدمت APIs خارجية زي Google Maps لتحديد مواقع العملاء."
    },
    new AnswerModel
    {
        Answer = "ما استخدمتش Git قبل كده لأنه مش مهم لإدارة الأكواد." // إجابة خطأ
    }
};

                //var questions = session.Questions.Select(q => q.OriginalQuestion).ToArray();
                var questions22 = questions2.Select(q => q.OriginalQuestion).ToArray();
                var report = await _interviewService.GenerateInterviewReport(answers2, questions22);

                TempData["InterviewReport"] = JsonConvert.SerializeObject(report);
                
                return Ok(new { success = true });
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
