using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TodoApi.DTO;
using TodoApi.Data;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
                .Include(t => t.DoNotDoItems)
                .Select(t => ItemToDTO(t))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems
                .Include(t => t.DoNotDoItems)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.DoNotDoItems)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            foreach (var doNotDoItem in todoItem.DoNotDoItems)
            {
                _context.DoNotDoItems.Remove(doNotDoItem);
            }

            foreach (var doNotDoItem in todoItemDTO.DoNotDoItems)
            {
                _context.DoNotDoItems.Add(doNotDoItem);
            }

            todoItem.DoNotDoItems = todoItemDTO.DoNotDoItems;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name,
                DoNotDoItems = new List<DoNotDoItem>(),
            };

            _context.TodoItems.Add(todoItem);

            for (var i = 0; i < todoItemDTO.DoNotDoItems.Count; i++)
            {
                var doNotDoItem = todoItemDTO.DoNotDoItems[i];
                var existingDoNotDoItem = await _context.DoNotDoItems
                    .Where(d => d.Id == doNotDoItem.Id)
                    .FirstOrDefaultAsync();
                if (existingDoNotDoItem == null)
                {
                    todoItem.DoNotDoItems.Add(doNotDoItem);
                }
                else
                {
                    todoItem.DoNotDoItems.Add(existingDoNotDoItem);
                }
            }

            await _context.SaveChangesAsync();

            // TODO: what is this?
            return CreatedAtAction(
                nameof(GetTodoItem),
                new
                {
                    id = todoItem.Id
                }, todoItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems
                .Include(t => t.DoNotDoItems)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            foreach (var doNotDoItem in todoItem.DoNotDoItems)
            {
                _context.DoNotDoItems.Remove(doNotDoItem);
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) =>
             _context.TodoItems.Any(e => e.Id == id);

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete,
                DoNotDoItems = todoItem.DoNotDoItems
            };
    }
}