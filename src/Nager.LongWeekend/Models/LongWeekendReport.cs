namespace Nager.LongWeekend
{
    /// <summary>
    /// Long Weekend Report
    /// </summary>
    public class LongWeekendReport
    {
        /// <summary>
        /// Start date
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Count of days
        /// </summary>
        public int DayCount { get { return this.EndDate.DayNumber - this.StartDate.DayNumber + 1; } }

        /// <summary>
        /// If an additional holiday needed
        /// </summary>
        public bool NeedBridgeDay { get; set; }

        /// <summary>
        /// The required bridge days
        /// </summary>
        public DateOnly[] BridgeDays { get; set; } = [];

        /// <summary>
        /// ToString - StartDate EndDate and DayCount
        /// </summary>
        /// <returns>Long weekend info formated</returns>
        public override string ToString()
        {
            return $"{this.StartDate} - {this.EndDate} ({this.DayCount})";
        }
    }
}
