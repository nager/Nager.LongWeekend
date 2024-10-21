namespace Nager.LongWeekend
{
    /// <summary>
    /// Long Weekend Calculator Interface
    /// </summary>
    public interface ILongWeekendCalculator
    {
        /// <summary>
        /// Calculate long weekends
        /// </summary>
        /// <param name="availableBridgeDays"></param>
        /// <returns>Set of long weekends for given public holidays</returns>
        IEnumerable<LongWeekendReport> Calculate(int availableBridgeDays = 1);
    }
}
