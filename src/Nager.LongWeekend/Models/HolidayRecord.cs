namespace Nager.LongWeekend.Models
{
    /// <summary>
    /// Holiday Record
    /// </summary>
    public class HolidayRecord
    {
        /// <summary>
        /// Date of the holiday
        /// </summary>
        public DateOnly Date {  get; set; }

        /// <summary>
        /// Name of the holiday
        /// </summary>
        public required string Name { get; set; }
    }
}
