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

        // POST: api/todos/upload-file (لرفع أي ملف)
        [HttpPost("upload-file")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var fileUrl = await _fileService.UploadAnyFileAsync(file);

            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest("Failed to upload file");

            return Ok(new
            {
                message = "File uploaded successfully",
                fileUrl = fileUrl
            });
        }

        // GET: api/todos/test
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new
            {
                message = "API is working! 🚀",
                timestamp = DateTime.UtcNow
            });
        }
    }
}