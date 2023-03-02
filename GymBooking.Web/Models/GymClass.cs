using System.ComponentModel.DataAnnotations;
namespace GymBooking.Web.Models
{
#nullable disable
    public class GymClass
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return StartTime + Duration; } }
        [Required]
        public string Description { get; set; }
            
        public bool isDatePassed { get; set; } = true;

        public string isBookedLabel { get; set; } = "Book";

        //Navigationspoperty, så aatt flera medlemmar kan delta i en GymKlass
        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; }

    }
}
