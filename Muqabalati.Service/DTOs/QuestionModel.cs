using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
    public class QuestionModel
    {

        public string LinkingPhrase { get; set; } // كلمة الربط بين الأسئلة
        public string OriginalQuestion { get; set; } // السؤال الأساسي
        public string RephrasedQuestion { get; set; } // إعادة الصياغة
        public string Explanation { get; set; } // التوضيح
    }

}
