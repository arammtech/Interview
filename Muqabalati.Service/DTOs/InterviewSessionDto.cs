using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
    public class InterviewSessionDto
    {
        public string ApplicantName { get; set; } = "علي"; // اسم المتقدم الافتراضي
        public string IntroText { get; set; } = "مرحباً، شكراً لانضمامك إلى المقابلة."; // نص المقدمة الافتراضي
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>
        {
            new QuestionModel
            {
                LinkingPhrase = "وبعد ذلك، دعنا ننتقل إلى السؤال التالي:",
                OriginalQuestion = "ما هي خبراتك السابقة في البرمجيات؟",
                RephrasedQuestion = "هل يمكنك أن تشرح لنا كيف تعاملت مع مشاريع برمجية؟",
                Explanation = "هذا السؤال يهدف إلى معرفة مدى خبراتك التقنية."
            },
            new QuestionModel
            {
                LinkingPhrase = "في نفس السياق، يمكننا أن نناقش:",
                OriginalQuestion = "كيف تعمل ضمن فريق؟",
                RephrasedQuestion = "هل يمكنك وصف دورك في فريق عمل سابق؟",
                Explanation = "نريد تقييم مدى قدرتك على التعاون والتواصل في بيئة عمل جماعية."
            }
        }; // قائمة افتراضية للأسئلة
        public string ConclusionText { get; set; } = "شكراً على وقتك ونتمنى لك التوفيق والان سننقلك الى صفحة النتيجة."; // نص الختام الافتراضي
    }
}
