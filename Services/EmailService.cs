using PerformanceSurvey.iServices;
using System.Net.Mail;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PerformanceSurvey.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _baseAddress;

        public EmailService()
        {
            _apiKey = "Your_Key"; // Replace with your actual API key
            _baseAddress = "https://example_emailAPI.com";
        }

        public async Task SendEmailAsync(string Recepient, string Title, string body)
        {
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(_baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                http.PreAuthenticate = true;
                http.Headers.Add("Authorization", $"Zoho-enczapikey {_apiKey}");

                var emailContent = new
                {
                    from = new
                    {
                        address = "example.com",
                        name = "your-Sender"
                    },
                    to = new[]
                    {
                new { email_address = new { address = Recepient, name = Recepient } }
            },
                    subject = Title,
                    htmlbody = body
                };

                var json = JObject.FromObject(emailContent).ToString();
                var encoding = new UTF8Encoding();
                var bytes = encoding.GetBytes(json);

                using (var requestStream = await http.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }

                using (var response = await http.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            var responseContent = await sr.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }
    }
    }

