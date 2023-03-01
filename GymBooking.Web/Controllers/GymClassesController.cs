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
using Microsoft.AspNetCore.Authorization;
using GymBooking.Web.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Http;

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
            if (_context.GymClass != null)
            {
                //var userId = _userManager.GetUserId(User);
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var gymClass= await _context.GymClass.Include(x=>x.AttendingMembers).ToListAsync();

                foreach(var a in gymClass)
                {
                  //  a.isDatePassed = true;
                    if(a.AttendingMembers.Any(x=>x.ApplicationUser == user) )
                    {
                        a.isBookedLabel = "unbook";    
                    }
                    else
                        a.isBookedLabel = "book";
                }
                return View(gymClass);
                // return View(await _context.GymClass.ToListAsync());
            }
            return Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
        }

        public async Task<IActionResult>MyBookedClasses()
        {

            if (_context.GymClass != null)
            {
                
                //var userId = _userManager.GetUserId(User);
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var gymClass = await _context.GymClass.Include(x => x.AttendingMembers).ToListAsync();
                var gymClass2 = await _context.GymClass.ToListAsync();
                foreach (var a in gymClass)
                {
                    //  a.isDatePassed = true;
                    if (a.AttendingMembers.Any(x => x.ApplicationUser == user))
                    {

                    }
                    else
                        gymClass2.Remove(a);
                }

                return View(nameof(MyBookedClasses),gymClass2);
                // return View(await _context.GymClass.ToListAsync());
            }
            return Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
        }

        public async Task<IActionResult> MyHistory()
        {

            if (_context.GymClass != null)
            {

                //var userId = _userManager.GetUserId(User);
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var gymClass = await _context.GymClass.Include(x => x.AttendingMembers).ToListAsync();
                var gymClass2 = await _context.GymClass.ToListAsync();
                foreach (var a in gymClass)
                {
                    //  a.isDatePassed = true;
                    if (a.AttendingMembers.Any(x => x.ApplicationUser == user))
                    {

                    }
                    else
                        gymClass2.Remove(a);
                }
                
                var gym = await _context.GymClass.ToListAsync();
                foreach (var a in gymClass2)
                {
                    if (a.StartTime>DateTime.Now)
                      gym.Remove(a);
                }

                return View(nameof(MyBookedClasses), gym);
                // return View(await _context.GymClass.ToListAsync());
            }
            return Problem("Entity set 'ApplicationDbContext.GymClass'  is null.");
        }



        [HttpPost]
        public async Task<IActionResult> IsChecked(IFormCollection formCollection)
        {
             bool chkeco = false;
    
        if (!string.IsNullOrEmpty(formCollection["chkeco"])) { chkeco = true; }

            var thisDatesThingy = await _context.GymClass.ToListAsync();

            if(chkeco==true)
            foreach (var date in thisDatesThingy)
            {
                if (date.StartTime >= DateTime.Now)
                    date.isDatePassed= true;
                else
                    date.isDatePassed= false;
            }
            if(chkeco==false)
                foreach(var date in thisDatesThingy)
                { date.isDatePassed= true; }

            TempData["success"] = "isChecked";
            //return View(nameof(Index),thisDatesThingy);

            foreach (var e in thisDatesThingy)
            {
                _context.Update(e);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null)
                return NotFound();

           // var userId = User.Identity.Name;
            var userId = _userManager.GetUserId(User);  // nu ska vi hitta medlemmen 
            if (userId == null) return NotFound();
            var attendClass = await _context.ApplicationUserGymClass.FindAsync(userId, id); //17:07

            //var thisClass = await _context.GymClass.FindAsync(id); //classID

            if (attendClass != null) //vi ska avboka om true
            {  
                TempData["success"] = "Avbokad";
                _context.ApplicationUserGymClass.Remove(attendClass);
            }
                else
                {
                //false: vi behöver göra en bokning! 
                int id2 = (int)id;
                var thisUser = await _userManager.GetUserAsync(User);
                var thisClass = await _context.GymClass.FindAsync(id);
                var booking = new ApplicationUserGymClass()
                   {
                    GymClassId = id2,
                    ApplicationUserId = userId,     //okklart om nedan behövs....
                    ApplicationUser= thisUser,
                    GymClass= thisClass
                   };

                _context.ApplicationUserGymClass.Add(booking);
                TempData["success"] = "Bokad";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
         
        }


        // GET: GymClasses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClass == null)
            {
                return NotFound();
            }

            //var gymClass = await _context.GymClass
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var gymClass = await _context.GymClass.Include(x => x.AttendingMembers)
                .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id); //Dessa rader är GULD!   =D  Går från GymClass till AppUserGymClass, sen till AppUser

            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
      
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
