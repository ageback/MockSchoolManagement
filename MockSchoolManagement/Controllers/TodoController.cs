using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]/[action]")]
    public class TodoController : ControllerBase
    {
        private readonly IRepository<TodoItem, long> _todoItemRepository;

        public TodoController(IRepository<TodoItem, long> todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetAll()
        {
            return await _todoItemRepository.GetAllListAsync();
        }

        
        /// <summary>
        /// 通过Id获取待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetById(long id)
        {
            var todoItem = await _todoItemRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (todoItem == null) { return NotFound(); }
            return todoItem;
        }
        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="id">待更新的事项Id</param>
        /// <param name="todoItem">待更新的TodoItem对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id,TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }
            await _todoItemRepository.UpdateAsync(todoItem);
            // 返回状态码204
            return NoContent();
        }

        /// <summary>
        /// 添加待办事项
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns></returns> 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            await _todoItemRepository.InsertAsync(todoItem);
            return CreatedAtAction(nameof(GetAll), new { id = todoItem.Id }, todoItem);
        }
        /// <summary>
        /// 删除指定Id的待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> Delete(long id)
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
