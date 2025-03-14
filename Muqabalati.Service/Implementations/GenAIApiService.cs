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
      string department,
      string interviewLanguage)
        {
            var introPrompt = GenerateIntroPrompt(applicantName, interviewerName, tone, topic, level, department, interviewLanguage);
            return await GenerateContent(apiKey, introPrompt);
        }

        public async Task<string> GenerateConclusionText(
            string apiKey,
            string applicantName,
            string tone,
            string interviewLanguage)
        {
            var conclusionPrompt = GenerateConclusionPrompt(applicantName, tone, interviewLanguage);
            return await GenerateContent(apiKey, conclusionPrompt);
        }


        public async Task<string> GenerateQuestionText(
            string apiKey,
            int questionNum,
            string topic,
            string level,
            string department,
            string tone,
            string terminologyLanguage,
            string interviewLanguage)
        {
            var questionPrompt = GenerateQuestionPrompt(questionNum, topic, level, department, tone,terminologyLanguage, interviewLanguage);
            return await GenerateContent(apiKey, questionPrompt);
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


        private string GenerateIntroPrompt(
            string applicantName,
            string interviewerName,
            string topic, 
            string tone,
            string level, 
            string department, 
            string interviewLanguage)
        {
            return $@"
                                  صِغ تحية ودية تُستخدم في بداية مقابلة مع متقدم. يجب أن تتضمن التحية:

                * الترحيب بالمتقدم بشكل لائق.

                * تعريف النظام أو الشخص الذي يجري المقابلة.

                * تقديم توجيهات بسيطة ومشجعة حول كيفية الإجابة.


                * إخباره بأن لديه وقتًا معينًا سيظهر أمامه للإجابة على كل سؤال. 

                * إخباره بأن لديه  ازرار  في الشاشة لتساعده لاعادة السؤال او للتوضيح او الانتقال الى السؤال التالي بصياغة ودية

                * اخبرة 'الان  المقابلة ستبدا' بصياغة ودية .



                تفاصيل السياق:

                * اللغة: {interviewLanguage}

                * اللهجة: {tone}

                * اسم المتقدم: {applicantName}

                * اسم المحاور: {interviewerName}

                * الموضوع: {topic}

                * المستوى المطلوب: {level}

                * القسم: {department}



                النص يجب أن يكون:
                * الحفاظ على أسلوب ودي ومهني.
                * واضحًا ومباشرًا.
                * خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة و  خاليا من علامات النحو وغيرها من العلامات التنصيص او النقاط او الفواصل او بدء سطر جديد
                * جاهزًا للاستخدام فورًا مع Web Speech AI.
";
        }

        private string GenerateConclusionPrompt(
            string applicantName,
            string tone,
            string interviewLanguage)
        {
            return $@"
                صِغ خاتمة ودية تُستخدم لإنهاء مقابلة مع متقدم. يجب أن تتضمن الخاتمة:

                * شكر المتقدم باسمه ({applicantName}) على وقته وجهوده أثناء المقابلة.
                * الإشارة إلى أنه سيتم نقله إلى صفحة النتائج خلال ثوانٍ قليلة، حيث يمكنه الاطلاع على تقييمه النهائي.
                * الحفاظ على أسلوب ودي ومشجع لإنهاء المقابلة بانطباع إيجابي.
                * استخدام اللهجة: {tone}.
                                تفاصيل السياق:

                * اللغة: {interviewLanguage}

                * اللهجة: {tone}

                * اسم المتقدم: {applicantName}


                النص يجب أن يكون:
                * الحفاظ على أسلوب ودي ومهني.
                * واضحًا ومباشرًا.
                * خاليًا من أي رموز أو تعليمات تحتاج إلى معالجة و  خاليا من علامات النحو وغيرها من العلامات التنصيص او النقاط او الفواصل او بدء سطر جديد
                * جاهزًا للاستخدام فورًا مع Web Speech AI.
    ";
        }

        private string GenerateQuestionPrompt(
          int questionNum,
          string topic,
          string level,
          string department,
          string tone,
          string terminologyLanguage,
          string interviewLanguage)
        {
            return $@"
                    صِغ مجموعة من {questionNum} أسئلة موجهة إلى متقدم في مقابلة. يجب أن تتضمن ما يلي:

                    1. إضافة كلمة تمهيدية تربط كل سؤال بالسؤال السابق 
                       (مثل: ""وبعد ذلك، دعنا ننتقل إلى..."" أو ""في نفس السياق، سأسأل...""أو ""السؤال الاول هو"" أو ""بالإضافة إلى ذلك..."").  
                       *يرجى تجنب استخدام كلمات أو عبارات تحتوي على إجابات إيجابية أو موافقة، مثل: ""طيب دلوقتي بعد ما اتكلمنا عن خبرتك السابقة"" أو أي كلمة تربط الحديث بخبرة معينة قم بذكر رقم السؤال في البداية مع ذكر كلمة الربط ""السؤال الاول هو"".*

                    2. صياغة السؤال الأساسي بدقة ووضوح.

                    3. إعادة صياغة للسؤال باستخدام كلمات مختلفة مع الحفاظ على معناه.  
                       *يُفضل إضافة أمثلة توضيحية إذا كان ذلك مناسبًا.*

                    4. تقديم تفسير أو توضيح للسؤال لمساعدة المتقدم على فهم المطلوب.  
                       *يُفضل إضافة أمثلة عملية إذا كانت متاحة.*

                    5. تقدير الوقت المطلوب للإجابة على السؤال بالدقائق.

            متطلبات الأسئلة:

                        * الأسئلة يجب أن تكون واضحة ومباشرة.
                        * ذات صلة بالسياق التالي:
                          * الموضوع: {topic}.
                          * اللغة: {interviewLanguage}.
                          * المستوى المطلوب: {level}.
                          * القسم: {department}.
                        * مكتوبة بلهجة {tone} مع استخدام المصطلحات التقنية باللغة {terminologyLanguage}.
                        * يجب أن تتبع الأسئلة التنسيق التالي:
                          <سؤال>
                          1. [كلمة الربط]
                          2. [السؤال الأصلي لاتقم باضافة نقط او فواصل في النص]
                          3. [إعادة الصياغة مع أمثلة إذا لزم الأمر  لاتقم باضافة نقط او فواصل في النص]
                          4. [ التوضيح مع أمثلة عملية إذا امكن  لاتقم باضافة نقط او فواصل في النص ]
                          5. [الوقت المحتمل للإجابة بالدقائق اعد فقط الرقم لكي يمكنني تحولية الى int]
                          </سؤال>

                      النص المطلوب يجب أن يلتزم بالمعايير التالية:

1. **وضوح النص ومباشرته:** 
   - يجب أن يكون النص بسيطًا وخاليًا من التعقيد.

2. **خلو النص من الرموز والعلامات غير الضرورية:**
   - لا يُسمح باستخدام علامات التنصيص مثل: ""` أو '""'"".
   - يجب تجنب النقاط الزائدة أو أي رموز أخرى او علامات النحو مثل الفاصلة وغيرها.

3. **التنسيق المعتمد:**
   - النص يجب أن يلتزم بالشكل التالي فقط:
     ```
     <سؤال>
     1. [كلمة الربط]
     2. [السؤال الأصلي]
     3. [إعادة الصياغة مع أمثلة إذا لزم الأمر]
     4. [التوضيح مع أمثلة عملية إذا أمكن]
     5. [الوقت المحتمل للإجابة بالدقائق اعد فقط الرقم لكي يمكنني تحويله إلى int]
     </سؤال>
     ```

4. **سهولة الاستخدام:** 
   - يجب أن يكون النص جاهزًا للاستخدام الفوري دون الحاجة إلى أي معالجة إضافية.

5. **استثناءات التنسيق:**
   - الاستثناءات الوحيدة المسموح بها هي تنسيقات الفقرات والعناصر داخل إطار: `<سؤال>` كما هو محدد أعلاه.

النص الناتج عن هذه المعايير سيكون ملائمًا لأي تطبيق تقني دون الحاجة إلى معالجة إضافية، وسيسهل العمل معه بشكل مباشر.
                
                        لا تحذف هذا الفورمات لكي استطيع فصل الاسئلة مع اجزاءها او الفواصل. 
                        النص يجب أن يكون جاهزًا للاستخدام فورًا مع Web Speech AI.
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
