using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;

namespace HTMLApi.Controllers
{
    [ApiController]
    [Route("api/v1/HTMLApi/[Controller]")]
    public class PageController : ControllerBase
    {
        private readonly ILogger<PageController> _logger;

        public PageController(ILogger<PageController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];

                var pathToSave = Directory.GetCurrentDirectory();
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    if (!fileName.Contains(".html"))
                    {
                        throw new Exception("Invalid File Format");
                    }

                    Random x = new Random();

                    fileName = fileName + "_" + x.Next(0, 10000000).ToString();

                    var fullPath = Path.Combine(pathToSave, fileName);

                    var dbPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("GetHTML/{FileName}")]
        public ContentResult GetHTML(string faturaNo)
        {
            var html = GetFile(faturaNo);

            return base.Content(html, "text/html");
        }

        [HttpGet("Download/{FileName}")]
        public async Task<IActionResult> Download(string FileName)
        {
            var fileName = System.IO.Path.GetFileName($@"{Directory.GetCurrentDirectory()}\{FileName}.html");
            var content = await System.IO.File.ReadAllBytesAsync($@"{Directory.GetCurrentDirectory()}\{FileName}.html");
            new FileExtensionContentTypeProvider()
                .TryGetContentType(fileName, out string contentType);
            return File(content, contentType, fileName);
        }

        protected string GetFile(string FileName)
        {
            var html = System.IO.File.ReadAllText($@"{Directory.GetCurrentDirectory()}\{FileName}.html");

            return html;
        }
    }
}
