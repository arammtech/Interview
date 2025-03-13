using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Muqabalati.Service.DTOs;
using Muqabalati.Service.Interfaces;
using Newtonsoft.Json;

namespace Muqabalati.Service.Implementations
{
    public class GenAIApiService : IGenAIApiService
    {
        private readonly HttpClient _httpClient;

        public GenAIApiService()
        {
            _httpClient = new HttpClient();
        }



        public async Task<string> GenerateIntroText(
      string apiKey,
      string applicantName,
      string interviewerName,
      string tone,
      string topic,
      string level,
      string department)
        {
            var introPrompt = GenerateIntroPrompt(applicantName, interviewerName, tone, topic, level, department);
            return await GenerateContent(apiKey, introPrompt);
        }

        public async Task<string> GenerateQuestionText(
            string apiKey,
            int questionNum,
            string topic,
            string level,
            string department,
            string tone,
            string language)
        {
            var questionPrompt = GenerateQuestionPrompt(questionNum, topic, level, department, tone, language);
            return await GenerateContent(apiKey, questionPrompt);
        }

        public async Task<string> GenerateConclusionText(
            string apiKey,
            string applicantName,
            string tone)
        {
            var conclusionPrompt = GenerateConclusionPrompt(applicantName, tone);
            return await GenerateContent(apiKey, conclusionPrompt);
        }


        // Method to send a prompt to the GenAIApi API and get the response
        public async Task<string> GenerateContent(string apiKey, string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            // Request body for the API
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            // Serialize request body to JSON
            string json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send POST request to the API
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Ensure success code; throw exception if not
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody; // Return the API response
            }
            catch (HttpRequestException ex)
            {
                // Log or handle HTTP-specific errors
                Console.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Log or handle JSON serialization/deserialization errors
                Console.WriteLine($"JSON Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }


        private string GenerateIntroPrompt(string applicantName, string interviewerName, string topic, string tone, string level, string department)
        {
            return $@"
                                      صِغ تحية ودية تُستخدم في بداية مقابلة مع متقدم. يجب أن تتضمن التحية:

                * الترحيب بالمتقدم بشكل لائق.
                * تعريف النظام أو الشخص الذي يجري المقابلة.
                * تقديم توجيهات بسيطة ومشجعة حول كيفية الإجابة.
                * الحفاظ على أسلوب ودي ومهني.
                * إخباره بأن لديه وقتًا معينًا سيظهر أمامه للإجابة على كل سؤال.
                * إخباره بأن  سنبدأ الأسئلة أو المقابلة.

                تفاصيل السياق:

                * اللغة: العربية
                * اللهجة: {tone}
                * اسم المتقدم: {applicantName}
                * اسم المحاور: {interviewerName}
                * الموضوع: {topic}
                * المستوى المطلوب: {level}
                * القسم: {department}

                النص يجب أن يكون:

                * واضحًا ومباشرًا.
                * خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة.
                * جاهزًا للاستخدام فورًا مع Web Speech AI.

                        ";
        }

        private string GenerateConclusionPrompt(string applicantName, string tone)
        {
            return $@"
صِغ خاتمة ودية تُستخدم لإنهاء مقابلة مع متقدم. يجب أن تتضمن الخاتمة:

* شكر المتقدم باسمه ({applicantName}) على وقته وجهوده أثناء المقابلة.
* الإشارة إلى أنه سيتم نقله إلى صفحة النتائج خلال ثوانٍ قليلة، حيث يمكنه الاطلاع على تقييمه النهائي.
* الحفاظ على أسلوب ودي ومشجع لإنهاء المقابلة بانطباع إيجابي.
* استخدام اللهجة: {tone}.

النص يجب أن يكون:

* واضحًا ومباشرًا.
* خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة.
* جاهزًا للاستخدام فورًا مع Web Speech AI.
    ";
        }

        private string GenerateQuestionPrompt(int questionNum, string topic, string level, string department, string tone, string language)
        {
            return $@"
        صِغ مجموعة من {questionNum} أسئلة موجهة إلى متقدم في مقابلة. يجب أن تتضمن:

        1. إضافة كلمة تمهيدية قبل كل سؤال لتربطه بالسؤال السابق 
           (مثل: ""وبعد ذلك، دعنا ننتقل إلى..."" أو ""في نفس السياق، سأسأل..."" 
           أو ""بالإضافة إلى ذلك،"" إلخ).
        2. صياغة السؤال الأساسي.
        3. إعادة صياغة للسؤال بصيغة مختلفة مع الحفاظ على معناه، 
           مع تقديم أمثلة توضيحية إذا لزم الأمر.
        4. توضيح أو تفسير للسؤال لمساعدة المتقدم على فهم المطلوب، 
           مع تقديم أمثلة عملية إذا أمكن.
        5. تقدير الوقت المحتمل للإجابة على السؤال بالدقائق.

        متطلبات الأسئلة:

        * الأسئلة يجب أن تكون واضحة ومباشرة.
        * ذات صلة بالسياق التالي:
            * الموضوع: {topic}.
            * المستوى المطلوب: {level}.
            * القسم: {department}.
        * مكتوبة بلهجة {tone} مع استخدام المصطلحات التقنية باللغة {language}.
        * يجب أن تتبع الأسئلة التنسيق التالي:
            <سؤال>
            1. [كلمة الربط]
            2. [السؤال الأصلي]
            3. [إعادة الصياغة مع أمثلة إذا لزم الأمر]
            4. [التوضيح مع أمثلة عملية إذا أمكن]
            5. [الوقت المحتمل للإجابة بالدقائق اعد فقط الرقم لكي يمكنني تحولية الى int]
            </سؤال>

        النص الناتج يجب أن يكون:

        * مكتوبًا بطريقة احترافية وجاهزًا للاستخدام مع Web Speech AI.
        * خاليًا من أي رموز أو تعليمات إضافية.
    ";
        }

        public async Task<List<QuestionModel>> ParseQuestions(string jsonResponse)
        {
            var questionsList = new List<QuestionModel>();

            try
            {
                var parsedResponse = JsonConvert.DeserializeObject<GenAIApiResponse>(jsonResponse);

                if (parsedResponse?.Candidates != null && parsedResponse.Candidates.Count > 0)
                {
                    string rawText = parsedResponse.Candidates[0].Content.Parts[0].Text;
                    string decodedText = System.Web.HttpUtility.HtmlDecode(rawText);
                    string[] splitQuestions = decodedText.Split(new[] { "<سؤال>", "</سؤال>" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var questionBlock in splitQuestions)
                    {
                        string[] parts = questionBlock.Split(new[] { "1.", "2.", "3.", "4.", "5." }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 6)
                        {
                            if (int.TryParse(parts[5].Trim(), out int estimatedTime))
                            {
                                var questionModel = new QuestionModel
                                {
                                    LinkingPhrase = parts[1].Trim(),
                                    OriginalQuestion = parts[2].Trim(),
                                    RephrasedQuestion = parts[3].Trim(),
                                    Explanation = parts[4].Trim(),
                                    EstimatedTimeMinutes = estimatedTime
                                };

                                questionsList.Add(questionModel);
                            }
                        }
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            return questionsList;
        }



        // Dispose HttpClient properly
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
