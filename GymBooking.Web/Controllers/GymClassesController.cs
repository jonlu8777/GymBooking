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

        //[HttpPost]
      //public async Task<IActionResult> RegisterUser([Bind("Id,Email,FirstName,LastName,Password")] ApplicationUser registerModel)
      //  public async Task<IActionResult> RegisterUser(int id)
      //public async Task<IActionResult> RegisterUser([Bind("Id,Email,FirstName,LastName,Password,ConfimPassword")] RegisterModel.InputModel registerModel)
      //  {

         




      //      //if (ModelState.IsValid)
      //      //{
      //      //    registerModel.Input.TimeOfRegistration = DateTime.Now;

      //      //var appUser = new ApplicationUser()
      //      //{
      //      //    FirstName = registerModel.Input.FirstName,
      //      //    LastName = registerModel.Input.LastName,
      //      //    Email = registerModel.Input.Email,
      //      //    PasswordHash = registerModel.Input.ConfirmPassword,
      //      //    TimeOfRegistration = DateTime.Now
      //      //};
      //      ////ID kan strula?! 

      //      //    _context.Add(registerModel);
      //      //    _context.ApplicationUser.Add(appUser);
      //      //    await _context.SaveChangesAsync();
      //      //    return RedirectToAction(nameof(Index));
      //      //}
      //      //return Problem("registration error");
      //  }


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

                ViewBag.Book = "Avbokad";
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

                ViewBag.Book = "Bokad";
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
