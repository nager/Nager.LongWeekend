namespace Nager.LongWeekend.Models
{
    public class DayOffReport
    {
        public bool IsDayOffPossible { get; set; }
        public DateOnly[] BridgeDays { get; set; }
        public int BridgeDayCount {  get; set; }
    }
}
