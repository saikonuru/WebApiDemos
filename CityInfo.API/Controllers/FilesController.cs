using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var filePath = ".\\Controllers\\Source Code.pdf";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            if (!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0 || file.ContentType != "application/pdf") // Adding null check
            {
                return BadRequest("No file or an invalid one has been inputted.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok("File has been uploaded successfully");
        }
    }
}
