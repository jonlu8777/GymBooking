using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymBooking.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Navigationsprperty, så att medlemmar kan boka flera GymPass

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName } {LastName}";

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime TimeOfRegistration { get; set; }

        public ICollection<ApplicationUserGymClass>? AttendedClasses { get; set; } = new List<ApplicationUserGymClass>();

    }
}
