using System.Net.Http.Headers;

var apiKey = "NEEDS TO BE SET";
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
