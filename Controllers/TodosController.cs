using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return await _context.Todos.ToListAsync();
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
        {
            todo.CreatedDate = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        // POST: api/todos/upload
        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file == null) return BadRequest("No file uploaded");

            var fileUrl = await _fileService.UploadFileAsync(file);
            return Ok(new { fileUrl });
        }
    }
}
