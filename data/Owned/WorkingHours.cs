using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    [Owned]
    public class WorkingHours
    {
        public bool? IsActiveMonday { get; set; }
        public TimeOnly? StartTimeMonday { get; set; }
        public TimeOnly? FinishTimeMonday { get; set; }
        public bool? IsActiveTuesday { get; set; }
        public TimeOnly? StartTimeTuesday { get; set; }
        public TimeOnly? FinishTimeTuesday { get; set; }
        public bool? IsActiveWednesday { get; set; }
        public TimeOnly? StartTimeWednesday { get; set; }
        public TimeOnly? FinishTimeWednesday { get; set; }
        public bool? IsActiveThursday { get; set; }
        public TimeOnly? StartTimeThursday { get; set; }
        public TimeOnly? FinishTimeThursday { get; set; }
        public bool? IsActiveFriday { get; set; }
        public TimeOnly? StartTimeFriday { get; set; }
        public TimeOnly? FinishTimeFriday { get; set; }
        public bool? IsActiveSaturday { get; set; }
        public TimeOnly? StartTimeSaturday { get; set; }
        public TimeOnly? FinishTimeSaturday { get; set; }
        public bool? IsActiveSunday { get; set; }
        public TimeOnly? StartTimeSunday { get; set; }
        public TimeOnly? FinishTimeSunday { get; set; }
    }
}
