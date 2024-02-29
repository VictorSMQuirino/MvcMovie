using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using NuGet.Packaging;

namespace MvcMovie.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly MvcMovieContext _context;

        public MovieController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            return _context.Movie != null ?
                        View(await _context.Movie.ToListAsync()) :
                        Problem("Entity set 'MvcMovieContext.Movie'  is null.");
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.Include(m => m.Artists)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewBag.Artists = movie.Artists;
            return View(movie);
        }

        // GET: Movie/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var studios = _context.Studio.ToList();
            var artists = _context.Artist.ToList();

            ViewData["StudioId"] = new SelectList(studios, "StudioId", "Name");
            ViewData["Artists"] = new SelectList(artists, "Id", "Name");

            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,StudioId")] Movie movie, string[] Artists)
        {
            if (ModelState.IsValid)
            {
                movie.Artists = new List<Artist>();
                var ids = Artists.Select(id => int.Parse(id)).ToList();
                var artistas = _context.Artist.Where(a => ids.Contains(a.Id)).ToList();
                movie.Artists.AddRange(artistas);
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var studios = _context.Studio.ToList();
            var artists = _context.Artist.ToList();

            ViewData["StudioId"] = new SelectList(studios, "StudioId", "Name");
            ViewData["Artists"] = new SelectList(artists, "Id", "Name");

            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,StudioId")] Movie movie, string[] Artists)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var movieToUpdate = await _context.Movie.Include(m => m.Artists).FirstOrDefaultAsync(m => m.Id == id);

                    var ids = Artists.Select(id => int.Parse(id)).ToList();
                    var artistas = _context.Artist.Where(a => ids.Contains(a.Id)).ToList();

                    if(movieToUpdate.Artists is not null && movieToUpdate.Artists.Any())
                    {
                        movieToUpdate.Artists.Clear();
                    }
                    
                    movieToUpdate.Artists.AddRange(artistas);
                    _context.Update(movieToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movie/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
