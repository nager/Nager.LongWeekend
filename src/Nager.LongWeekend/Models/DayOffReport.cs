namespace Nager.LongWeekend.Models
{
    /// <summary>
    /// Day Off Report
    /// </summary>
    public class DayOffReport
    {
        /// <summary>
        /// Is a day off possible
        /// </summary>
        public bool IsDayOffPossible { get; set; }

        /// <summary>
        /// Bridge days
        /// </summary>
        public DateOnly[] BridgeDays { get; set; } = [];

        /// <summary>
        /// Bridge day count
        /// </summary>
        public int BridgeDayCount {  get; set; }
    }
}
