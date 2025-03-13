using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Domain.Global
{
    public class QuestionRequest
    {
        
        public string ApplicantName { get; set; }
        public string InterviewerName { get; set; }
        public string Topic { get; set; } = ".NET";
        public string Level { get; set; } = "مبتدئ";
        public string Department { get; set; } = "تقنية معلومات";
        public string Tone { get; set; } = "مصري";
        public string Language { get; set; } = "English";
    }
}
