using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcPractice2.Data;
using MvcPractice2.Models;

namespace MvcPractice2.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcPractice2Context _context;

        public MoviesController(MvcPractice2Context context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helloWorld = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helloWorld == null)
            {
                return NotFound();
            }

            return View(helloWorld);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] HelloWorld helloWorld)
        {
            if (ModelState.IsValid)
            {
                _context.Add(helloWorld);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(helloWorld);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helloWorld = await _context.Movie.FindAsync(id);
            if (helloWorld == null)
            {
                return NotFound();
            }
            return View(helloWorld);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] HelloWorld helloWorld)
        {
            if (id != helloWorld.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(helloWorld);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HelloWorldExists(helloWorld.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(helloWorld);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helloWorld = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helloWorld == null)
            {
                return NotFound();
            }

            return View(helloWorld);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var helloWorld = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(helloWorld);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HelloWorldExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
