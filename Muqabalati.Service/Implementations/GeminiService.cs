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
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;

        public GeminiService()
        {
            _httpClient = new HttpClient();
        }



        public async Task<string> GenerateIntroText(string apiKey, string applicantName, string interviewerName, string topic, string level, string department)
        {
            var introPrompt = GenerateIntroPrompt(applicantName, interviewerName, topic, level, department);
            return await GenerateContent(apiKey, introPrompt);
        }

        public async Task<string> GenerateQuestionText(string apiKey,int questionNum, string topic, string level, string department, string tone, string language)
        {
            var questionPrompt = GenerateQuestionPrompt(questionNum,topic, level, department, tone, language);
            return await GenerateContent(apiKey, questionPrompt);
        }

        public async Task<string> GenerateConclusionText(string apiKey)
        {
            var conclusionPrompt = GenerateConclusionPrompt();
            return await GenerateContent(apiKey, conclusionPrompt);
        }

        // Method to send a prompt to the Gemini API and get the response
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


        private string GenerateIntroPrompt(string applicantName, string interviewerName, string topic, string level, string department)
        {
            return $@"
                        صِغ تحية ودية تُستخدم في بداية مقابلة مع متقدم. يجب أن تتضمن التحية:
                        - الترحيب بالمتقدم بشكل لائق.
                        - تعريف النظام أو الشخص الذي يجري المقابلة.
                        - تقديم توجيهات بسيطة ومشجعة حول كيفية الإجابة.
                        - الحفاظ على أسلوب ودي ومهني.
                        - أخبره بأن لديه وقتًا معينًا سيظهر في امامة للإجابة على كل سؤال.

                        تفاصيل السياق:
                        - اسم المتقدم: {applicantName}
                        - اسم المحاور: {interviewerName}
                        - الموضوع: {topic}
                        - المستوى المطلوب: {level}
                        - القسم: {department}

                        النص يجب أن يكون:
                        - واضحًا ومباشرًا.
                        - خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة.
                        - جاهزًا للاستخدام فورًا مع Web Speech AI.
                        ";
        }

        private string GenerateConclusionPrompt()
        {
            return $@"
                صِغ خاتمة ودية تُستخدم لإنهاء مقابلة مع متقدم. يجب أن تتضمن الخاتمة:
                - شكر المتقدم باسمه على وقته وجهوده أثناء المقابلة.
                - الإشارة إلى وجود زر النتيجة، مع توجيه المتقدم للضغط عليه لرؤية التقييم النهائي.
                - الحفاظ على أسلوب ودي ومشجع لإنهاء المقابلة بانطباع إيجابي.

                النص يجب أن يكون:
                - واضحًا ومباشرًا.
                - خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة.
                - جاهزًا للاستخدام فورًا مع Web Speech AI.
                ";
        }

        //private string GenerateQuestionPrompt(int questionNum, string topic, string level, string department, string tone, string language)
        //{
        //    return $@"
        //            صِغ مجموعة من {questionNum} أسئلة موجهة إلى متقدم في مقابلة. يجب أن تكون الأسئلة:
        //            - واضحة ومباشرة.
        //            - ذات صلة بالسياق المحدد أدناه.
        //            - تحتوي على حلقة وصل بين كل سؤال وآخر لجعل الأسئلة تبدو مترابطة (مثال: ""بعد الإجابة على هذا السؤال، سأطرح عليك سؤالاً يتعلق بـ..."").
        //            - مكتوبة بلهجة {tone} مع استخدام المصطلحات التقنية والبرمجية باللغة {language}.
        //            - مفصولة باستخدام العلامتين التاليين:
        //                - بداية السؤال: <سؤال>
        //                - نهاية السؤال: </سؤال>

        //            تفاصيل السياق:
        //            - الموضوع: {topic}
        //            - المستوى المطلوب: {level}
        //            - القسم: {department}

        //            النص يجب أن يكون:
        //            - مكتوبًا باللهجة المحددة وبأسلوب احترافي.
        //            - خاليًا من أي رموز أو علامات إضافية.
        //            - مهيئًا للتعامل مع Web Speech AI دون معالجة إضافية.
        //    ";
        //}


        private string GenerateQuestionPrompt(int questionNum, string topic, string level, string department, string tone, string language)
        {
            return $@"
                    صِغ مجموعة من {questionNum} أسئلة موجهة إلى متقدم في مقابلة. يجب أن تتضمن:
                    1. إضافة كلمة تمهيدية قبل كل سؤال لتربطه بالسؤال السابق (مثل: ""وبعد ذلك، دعنا ننتقل إلى..."" أو ""في نفس السياق، سأسأل..."" أو ""بالإضافة إلى ذلك،"" إلخ).
                    2. صياغة السؤال الأساسي.
                    3. إعادة صياغة للسؤال بصيغة مختلفة مع الحفاظ على معناه.
                    4. توضيح أو تفسير للسؤال لمساعدة المتقدم.

                    متطلبات الأسئلة:
                    - الأسئلة يجب أن تكون واضحة ومباشرة.
                    - ذات صلة بالسياق التالي:
                        - الموضوع: {topic}.
                        - المستوى المطلوب: {level}.
                        - القسم: {department}.
                    - مكتوبة بلهجة {tone} مع استخدام المصطلحات التقنية باللغة {language}.
                    - مفصولة باستخدام العلامتين:
                        - بداية السؤال: <سؤال>
                        - نهاية السؤال: </سؤال>
                    النص الناتج يجب أن يكون:
                    - مكتوبًا بطريقة احترافية وجاهزًا للاستخدام مع Web Speech AI.
                    - خاليًا من أي رموز أو تعليمات إضافية.
                    ";
        }

        public Task<List<QuestionModel>> ParseQuestions(string jsonResponse)
        {
            var questionsList = new List<QuestionModel>();

            try
            {
                // تحويل النص الناتج إلى كائن JSON
                var parsedResponse = JsonConvert.DeserializeObject<GeminiResponse>(jsonResponse);

                if (parsedResponse?.Candidates != null && parsedResponse.Candidates.Count > 0)
                {
                    // الحصول على النصوص
                    string rawText = parsedResponse.Candidates[0].Content.Parts[0].Text;

                    // فك الترميز
                    string decodedText = System.Web.HttpUtility.HtmlDecode(rawText);

                    // تقسيم النص باستخدام العلامات <سؤال> و </سؤال>
                    string[] splitQuestions = decodedText.Split(new[] { "<سؤال>", "</سؤال>" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var questionBlock in splitQuestions)
                    {
                        // تقسيم السؤال إلى أجزائه: كلمة الربط، الصيغة الأساسية، إعادة الصياغة، التوضيح
                        string[] parts = questionBlock.Split(new[] { "1.", "2.", "3.", "4." }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 4)
                        {
                            // إنشاء كائن السؤال وإضافته إلى القائمة
                            var questionModel = new QuestionModel
                            {
                                LinkingPhrase = parts[0].Trim(), // كلمة الربط
                                OriginalQuestion = parts[1].Trim(),
                                RephrasedQuestion = parts[2].Trim(),
                                Explanation = parts[3].Trim()
                            };

                            questionsList.Add(questionModel);
                        }
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            // تحويل القائمة إلى Task
            return Task.FromResult(questionsList);
        }


        // Dispose HttpClient properly
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
