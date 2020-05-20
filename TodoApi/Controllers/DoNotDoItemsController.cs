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
    public class DoNotDoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public DoNotDoItemsController(TodoContext context)
        {
            _context = context;
        }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoNotDoItem>>> GetDoNotDoItems()
    {
        return await _context.DoNotDoItems.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoNotDoItem>> GetDoNotDoItem(long id)
    {
        var doNotDoItem = await _context.DoNotDoItems.FindAsync(id);

        if (doNotDoItem == null)
        {
            return NotFound();
        }

        return doNotDoItem;

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoNotDoItem(long id, DoNotDoItem doNotDoItem)
    {
        if (id != doNotDoItem.Id)
        {
            return BadRequest();
        }

        var newDoNotDoItem = await _context.DoNotDoItems.FindAsync(id);
        if (newDoNotDoItem == null)
        {
            return NotFound();
        }

        newDoNotDoItem.Name = doNotDoItem.Name;
        newDoNotDoItem.DidDo = doNotDoItem.DidDo;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!DoNotDoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> CreateDoNotDoItem(DoNotDoItem doNotDoItem)
    {
        var newDoNotDoItem = new DoNotDoItem
        {
            DidDo = doNotDoItem.DidDo,
            Name = doNotDoItem.Name
        };

        _context.DoNotDoItems.Add(newDoNotDoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDoNotDoItem),
            new { id = newDoNotDoItem.Id },
            newDoNotDoItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoNotdoItem(long id)
    {
        var doNotDoItem = await _context.DoNotDoItems.FindAsync(id);

        if (doNotDoItem == null)
        {
            return NotFound();
        }

        _context.DoNotDoItems.Remove(doNotDoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DoNotDoItemExists(long id) =>
         _context.DoNotDoItems.Any(e => e.Id == id);
    }

}