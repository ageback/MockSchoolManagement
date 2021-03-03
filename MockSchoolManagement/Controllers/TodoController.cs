using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly IRepository<TodoItem, long> _todoItemRepository;

        public TodoController(IRepository<TodoItem, long> todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        // GET:api/Todo
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetTodo()
        {
            return await _todoItemRepository.GetAllListAsync();
        }

        
        // GET:api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _todoItemRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (todoItem == null) { return NotFound(); }
            return todoItem;
        }
        // PUT:api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id,TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }
            await _todoItemRepository.UpdateAsync(todoItem);
            // 返回状态码204
            return NoContent();
        }

        // POST:api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            await _todoItemRepository.InsertAsync(todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _todoItemRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }
            await _todoItemRepository.DeleteAsync(todoItem);
            return todoItem;
        }
    }
}
