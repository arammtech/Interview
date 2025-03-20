namespace Muqabalati.Service.DTOs
{
    public class InterviewRequestDto
    {

        /// <summary>
        /// اسم المتقدم للمقابلة.
        /// </summary>
        public string ApplicantName { get; set; } = "ميري";

        /// <summary>
        /// اسم المحاور.
        /// </summary>
        public string InterviewerName { get; set; } = "جون";
        //public string InterviewerName { get; set; } = "سامر";

        /// <summary>
        /// موضوع المقابلة (مثل موضوع تقني محدد).
        /// </summary>
        public string Topic { get; set; } = ".Net Developer";

        /// <summary>
        /// مستوى المهارات المطلوب (مثال: مبتدئ، متوسط، خبير).
        /// </summary>
        public string SkillLevel { get; set; } = "Junior";

        /// <summary>
        /// اللهجة أو الأسلوب الذي سيتم به توجيه الأسئلة.
        /// </summary>
        public string Tone { get; set; } = "امريكي";

        /// <summary>
        /// لغة المصطلحات المستخدمة (مثل الإنجليزية).
        /// </summary>
        public string TerminologyLanguage { get; set; } = "الانجليزي";

        /// <summary>
        /// لغة المقابلة
        /// </summary>
        public string InterviewLanguage { get; set; } = "الانجليزية";

        /// <summary>
        /// عدد الأسئلة المطلوبة.
        /// </summary>
        public int QuestionCount { get; set; } = 5;

        public List<string>? Topics = new List<string>
            {
                "Machine Learning Engineer","Software Developer",".Net Developer","Full-Stack Developer","UI/UX Designer","Graphic Designer","Full-Stack Developer"
            };

    }
}
