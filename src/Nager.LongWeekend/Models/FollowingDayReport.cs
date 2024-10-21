namespace Nager.LongWeekend.Models
{
    internal class FollowingDayReport
    {
        internal int DayCount { get; set; }
        internal int BridgeDayCount { get; set; }
        internal bool BridgeDayRequired { get; set; }
        internal DateOnly[] BridgeDays { get; set; } = [];
    }
}
