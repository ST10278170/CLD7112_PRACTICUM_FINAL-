
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriStreamIoT.Models;
using AgriStreamIoT.Data;

public class AppUsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public AppUsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: APPUSERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.AppUsers.ToListAsync());
    }

    // GET: APPUSERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appuser = await _context.AppUsers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (appuser == null)
        {
            return NotFound();
        }

        return View(appuser);
    }

    // GET: APPUSERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: APPUSERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Username,Password,Role")] AppUser appuser)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appuser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(appuser);
    }

    // GET: APPUSERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appuser = await _context.AppUsers.FindAsync(id);
        if (appuser == null)
        {
            return NotFound();
        }
        return View(appuser);
    }

    // POST: APPUSERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Username,Password,Role")] AppUser appuser)
    {
        if (id != appuser.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(appuser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppUserExists(appuser.Id))
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
        return View(appuser);
    }

    // GET: APPUSERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appuser = await _context.AppUsers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (appuser == null)
        {
            return NotFound();
        }

        return View(appuser);
    }

    // POST: APPUSERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var appuser = await _context.AppUsers.FindAsync(id);
        if (appuser != null)
        {
            _context.AppUsers.Remove(appuser);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AppUserExists(int? id)
    {
        return _context.AppUsers.Any(e => e.Id == id);
    }
}
