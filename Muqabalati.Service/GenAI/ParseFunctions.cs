using Muqabalati.Service.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.GenAI
{
    public static class ParseFunctions
    {
        public static async Task<InterviewReportDto> ParseInterviewReport(string jsonResponse)
        {
            var report = new InterviewReportDto
            {
                Recommendations = new List<RecommendationDto>()
            };

            try
            {
                // تحليل JSON إلى النص الخام
                var parsedResponse = JsonConvert.DeserializeObject<GenAIApiResponse>(jsonResponse);

                if (parsedResponse?.Candidates != null && parsedResponse.Candidates.Count > 0)
                {
                    string rawText = parsedResponse.Candidates[0].Content.Parts[0].Text;

                    // استخراج القسم بين <تقييم> و </تقييم>
                    string[] parts = rawText.Split(new[] { "<تقييم>", "</تقييم>" }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 0)
                    {
                        string evaluationContent = parts[0];

                        // استخراج الإجابات الصحيحة، الخاطئة، ونسبة النجاح
                        string[] assessmentValues = evaluationContent.Split(new[] { "1.", "2.", "3." ,"4."}, StringSplitOptions.RemoveEmptyEntries);

                        if (assessmentValues.Length >= 3)
                        {
                            // استخدام TryParse للتحقق من صحة القيم العددية
                            if (int.TryParse(assessmentValues[1].Trim(), out int correctAnswers))
                            {
                                report.CorrectAnswers = correctAnswers;
                            }
                            else
                            {
                                Console.WriteLine("Error parsing CorrectAnswers value.");
                            }

                            if (int.TryParse(assessmentValues[2].Trim(), out int failAnswers))
                            {
                                report.FailAnswers = failAnswers;
                            }
                            else
                            {
                                Console.WriteLine("Error parsing FailAnswers value.");
                            }

                            if (int.TryParse(assessmentValues[3].Trim(), out int successRate))
                            {
                                report.GPA = successRate; // نسبة النجاح
                            }
                            else
                            {
                                Console.WriteLine("Error parsing GPA/SuccessRate value.");
                            }
                        }

                        // استخراج قسم التوصيات
                        if (evaluationContent.Contains("<توصيات>") && evaluationContent.Contains("</توصيات>"))
                        {
                            string[] recommendationsSection = evaluationContent.Split(new[] { "<توصيات>", "</توصيات>" }, StringSplitOptions.RemoveEmptyEntries);

                            if (recommendationsSection.Length > 0)
                            {
                                string recommendationsContent = recommendationsSection[1];
                                string[] recommendationBlocks = recommendationsContent.Split(new[] { "<توصية>", "</توصية>" }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var block in recommendationBlocks)
                                {
                                    string[] recommendationParts = block.Split(new[] { "<نص>", "</نص>", "<مصدر>", "</مصدر>" }, StringSplitOptions.RemoveEmptyEntries);

                                    if (recommendationParts.Length >= 2)
                                    {
                                        report.Recommendations.Add(new RecommendationDto
                                        {
                                            Recommendation = recommendationParts[1].Trim(),
                                            Source = recommendationParts[3].Trim()
                                        });
                                    }
                                    else
                                    {
                                     //   Console.WriteLine("Invalid recommendation format found.");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                      //  Console.WriteLine("No valid <تقييم> sections found in the response.");
                    }
                }
                else
                {
                    Console.WriteLine("No candidates or content found in the response.");
                }
            }
            catch (JsonException ex)
            {
                //Console.WriteLine($"JSON Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            return report;
        }



        public static async Task<List<QuestionModel>> ParseQuestions(string jsonResponse)
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

    }
}
