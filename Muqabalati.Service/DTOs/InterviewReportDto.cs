using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
    public class InterviewReportDto
    {

        public int FailAnswers { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
        public int GPA { get; set; }

        public List<RecommendationDto>? Recommendations { get; set; } = new List<RecommendationDto>();

    }
}
