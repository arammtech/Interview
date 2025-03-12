using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Domain.Global
{
    public class InterviewSession
    {
        //public int Id { get; set; }
        public List<string> Questions { get; set; }
        public string Evaluation { get; set; }
        public string Conclusion { get; set; }
        public List<string> UserResponses { get; set; } = new List<string>();
    }
}
