using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Onspring.API.SDK;
using Onspring.API.SDK.Models;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var result = await SaveFile(configuration.GetSection("ApiKey").Value, 123, 6989, "./test-attachment.txt");
Console.WriteLine(result.ToString());
await SaveFileUsingSdk();
await SaveFileWithoutUsingSdk();

async Task<int> SaveFile(string apiKey, int recordId, int fieldId, string filePath)
{

    if (File.Exists(filePath) && !string.IsNullOrEmpty(apiKey))
    {

        var requestUri = "https://api.onspring.com/Files";

        var fileName = Path.GetFileName(filePath);

        //var contentType = System.Web.MimeMapping.GetMimeMapping(fileName);
        var contentType = "plain/text";

        var stream = File.OpenRead(filePath);

        var streamContent = new StreamContent(stream);

        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);



        var multiPartContent = new MultipartFormDataContent

        {

            { streamContent, "File", fileName },

            { new StringContent(recordId.ToString()), "RecordId" },

            { new StringContent(fieldId.ToString()), "FieldId" },

            { new StringContent(""), "Notes" },

        };



        var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)

        {

            Content = multiPartContent,

        };



        httpRequest.Headers.Add("x-apikey", apiKey);

        httpRequest.Headers.Add("x-api-version", "2");

        var httpClient = new HttpClient();

        var httpResponse = await httpClient.SendAsync(httpRequest);

        return (httpResponse.StatusCode == System.Net.HttpStatusCode.Created) ? 1 : 0;

    }

    return 0;

}


// Not using SDK
async Task SaveFileWithoutUsingSdk()
{
    var apiKey = configuration.GetSection("ApiKey").Value;
    var requestUri = "https://api.onspring.com/Files";
    var fileName = "test-attachment.txt";
    var filePath = @$"./{fileName}";
    var contentType = "plain/txt";

    var stream = File.OpenRead(filePath);
    var streamContent = new StreamContent(stream);
    streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

    var multiPartContent = new MultipartFormDataContent
{
    { streamContent, "file", fileName },
    { new StringContent("123"), "RecordId" },
    { new StringContent("6989"), "FieldId" },
    { new StringContent("This is a test"), "Notes" },
};

    var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
    {
        Content = multiPartContent,
    };

    httpRequest.Headers.Add("x-apikey", apiKey);

    var httpClient = new HttpClient();

    var httpResponse = await httpClient.SendAsync(httpRequest);

    Console.WriteLine(httpResponse.StatusCode);
}

// Using SDK
async Task SaveFileUsingSdk()
{
    var apiKey = configuration.GetSection("ApiKey").Value;
    var baseUrl = "https://api.onspring.com";

    var fileName = "test-attachment.txt";
    var filePath = @$"./{fileName}";
    var contentType = "plain/txt";

    var client = new OnspringClient(baseUrl, apiKey);

    var request = new SaveFileRequest
    {
        RecordId = 123,
        FieldId = 6989,
        Notes = "This is a test",
        FileName = fileName,
        FileStream = File.OpenRead(filePath),
        ContentType = contentType,
    };

    var response = await client.SaveFileAsync(request);

    Console.WriteLine(response.StatusCode);
}