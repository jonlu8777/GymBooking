using Microsoft.AspNetCore.Identity;

namespace GymBooking.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Navigationsprperty, så att medlemmar kan boka flera GymPass
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }


    }
}
