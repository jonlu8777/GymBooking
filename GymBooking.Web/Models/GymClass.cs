namespace GymBooking.Web.Models
{
#nullable disable
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; }

        //Navigationspoperty, så aatt flera medlemmar kan delta i en GymKlass
        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; }

    }
}
