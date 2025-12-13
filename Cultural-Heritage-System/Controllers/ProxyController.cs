using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

[ApiController]
[Route("api/proxy")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly string _AIUrl;

    public ProxyController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _AIUrl = configuration["AIUrl:AIUrl"]
                ?? throw new ArgumentNullException("AIUrl:AIUrl is required");
        _http = factory.CreateClient();
    }

    [HttpPost("predict")]
    public async Task<IActionResult> Predict()
    {
        var query = HttpContext.Request.QueryString.Value ?? "";

        var targetUrl = _AIUrl + query;

        if (Request.Form.Files.Count == 0)
        {
            return BadRequest("Không nhận được file từ FE");
        }

        var file = Request.Form.Files["file"];
        if (file == null)
        {
            return BadRequest("Thiếu file 'file'");
        }

        var form = new MultipartFormDataContent();
        form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

        try
        {
            var response = await _http.PostAsync(targetUrl, form);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }

            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
