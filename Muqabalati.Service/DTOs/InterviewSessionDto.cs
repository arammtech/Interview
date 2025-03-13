using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
   public class InterviewSessionDto
    {
        public string ApplicantName { get; set; }
        public string IntroText { get; set; }
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
        public string ConclusionText { get; set; }
    }
}
