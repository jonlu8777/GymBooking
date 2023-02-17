namespace GymBooking.Web.Models
{
    public class ApplicationUserGymClass
    {

#nullable disable
        public string ApplicationUserId { get; set; }
        public int GymClassId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }
    }
}
