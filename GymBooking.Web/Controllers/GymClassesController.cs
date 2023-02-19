using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBooking.Web.Data;
using GymBooking.Web.Models;
using Microsoft.AspNetCore.Identity;
using NuGet.Versioning;

namespace GymBooking.Web.Controllers
{
    public class GymClassesController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager= userManager;
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
              return _context.GymClass != null ? 
                          View(await _context.GymClass.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null)
                return NotFound();

           // var userId = User.Identity.Name;
            var userId = _userManager.GetUserId(User);  // nu ska vi hitta medlemmen 
            if (userId == null) return NotFound();
            var thisClass = await _context.ApplicationUserGymClass.FindAsync(userId, id); //17:07

            //var thisClass = await _context.GymClass.FindAsync(id); //classID

            if (thisClass != null) //vi ska avboka om true
            {
            TempData["success"] = "Avbokad";
                _context.ApplicationUserGymClass.Remove(thisClass);
            }
                else
                {
                //false: vi behöver göra en bokning! 
                int id2 = (int)id;
                var thisUser = await _userManager.GetUserAsync(User);
                var booking = new ApplicationUserGymClass()
                   {
                    GymClassId = id2,
                    ApplicationUserId = userId
                   };

                _context.ApplicationUserGymClass.Add(booking);
                TempData["success"] = "Bokad";
            }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            //}
        }
//        var attending = await _context.AppUserGymClass.FindAsync(userId, id);

//            if(attending == null)
//            {
//                var booking = new ApplicationUserGymClass
//                {
//                    ApplicationUserId = userId,
//                    GymClassId = (int)id
//                };

//        _context.AppUserGymClass.Add(booking);
//            }
//            else
//            {
//                _context.AppUserGymClass.Remove(attending);
//            }

//await _context.SaveChangesAsync();

//return RedirectToAction("Index");
           
//        }

// GET: GymClasses/Details/5
public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClass == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
            }
            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClass.Remove(gymClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
          return (_context.GymClass?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
