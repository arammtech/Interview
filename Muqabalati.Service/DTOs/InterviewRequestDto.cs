using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
    public class InterviewRequestDto
    {
        public string ApplicantName { get; set; }
        public string InterviewerName { get; set; }
        public string Topic { get; set; }
        public string Level { get; set; }
        public string Department { get; set; } 
        public string Tone { get; set; }
        public string Language { get; set; }
        public int QuestionCount { get; set; }
    }
}
