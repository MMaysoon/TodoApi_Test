using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FileService _fileService;

        public TodosController(AppDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            return Ok(todos);
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
        {
            if (todo == null)
                return BadRequest("Todo data is required");

            todo.CreatedDate = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Todo created successfully",
                todo = todo
            });
        }

        // POST: api/todos/upload-image (لرفع الصور)
        [HttpPost("upload-image")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // تحقق من نوع الملف (صور فقط)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Only image files are allowed");

            var fileUrl = await _fileService.UploadFileAsync(file);

            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest("Failed to upload file");

            return Ok(new
            {
                message = "Image uploaded successfully",
                fileUrl = fileUrl
            });
        }

        // POST: api/todos/upload-pdf (مخصوص لـ PDF)
        [HttpPost("upload-pdf")]
        public async Task<ActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // تحقق من إن الملف PDF
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".pdf")
                return BadRequest("Only PDF files are allowed");

            var fileUrl = await _fileService.UploadPdfFileAsync(file);

            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest("Failed to upload PDF");

            return Ok(new
            {
                message = "PDF uploaded successfully",
                fileUrl = fileUrl,
                downloadUrl = fileUrl + "&download=1"  // إضافة parameter للتحميل
            });
        }

        // POST: api/todos/upload-document (لأي ملف وثائق)
        [HttpPost("upload-document")]
        public async Task<ActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // السماح بأنواع ملفات متعددة
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Only document files are allowed (PDF, DOC, DOCX, TXT)");

            var fileUrl = await _fileService.UploadPdfFileAsync(file);

            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest("Failed to upload document");

            return Ok(new
            {
                message = "Document uploaded successfully",
                fileUrl = fileUrl,
                fileName = file.FileName
            });
        }

        // GET: api/todos/download-pdf/{fileName} (لتحميل الـ PDF)
        [HttpGet("download-pdf/{fileName}")]
        public ActionResult DownloadPdf(string fileName)
        {
            try
            {
                // بناء الـ URL الصحيح للتحميل
                var downloadUrl = $"https://res.cloudinary.com/your_cloud_name/raw/upload/fl_attachment/{fileName}";

                return Ok(new
                {
                    downloadUrl = downloadUrl,
                    directLink = downloadUrl + "?download=1"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}