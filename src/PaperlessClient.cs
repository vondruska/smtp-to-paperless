using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace SmtpToPaperless
{
    public class PaperlessClient : IPaperlessClient
    {
        private readonly HttpClient _httpClient;
        public PaperlessClient(IHttpClientFactory httpClientFactory, IOptions<Configuration> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = options.Value.PaperlessBaseUrl;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Value.PaperlessUsername + ":" + options.Value.PaperlessPassword)));
        }

        public async Task UploadFileAsync(string fileName, Stream fileToUpload, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/documents/post_document/");
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(fileToUpload), "document", fileName }
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}