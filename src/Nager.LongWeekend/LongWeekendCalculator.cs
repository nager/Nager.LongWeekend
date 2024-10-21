using Nager.LongWeekend.Models;

namespace Nager.LongWeekend
{
    /// <summary>
    /// Long Weekend Calculator
    /// </summary>
    public class LongWeekendCalculator : ILongWeekendCalculator
    {
        private readonly IEnumerable<HolidayRecord> _holidayRecords;
        private readonly IEnumerable<DayOfWeek> _weekendDays;

        /// <summary>
        /// Long Weekend Calculator
        /// </summary>
        /// <param name="holidayRecords"></param>
        /// <param name="weekendDays"></param>
        public LongWeekendCalculator(
            IEnumerable<HolidayRecord> holidayRecords,
            IEnumerable<DayOfWeek> weekendDays)
        {
            this._holidayRecords = holidayRecords;
            this._weekendDays = weekendDays;
        }

        /// <inheritdoc/>
        public IEnumerable<LongWeekendReport> Calculate(
            int availableBridgeDays = 1)
        {
            var items = new List<LongWeekendReport>();

            void AddUniqueLongWeekend(LongWeekendReport longWeekend)
            {
                if (items.Any(item => longWeekend.StartDate >= item.StartDate && longWeekend.EndDate <= item.EndDate))
                {
                    return;
                }

                items.Add(longWeekend);
            }

            foreach (var holiday in this._holidayRecords)
            {
                var previousDayResult = this.AnalyzeFollowingDays(holiday.Date, DateSearchDirection.Backward, availableBridgeDays);
                var nextDayResult = this.AnalyzeFollowingDays(holiday.Date, DateSearchDirection.Forward, availableBridgeDays);

                // No days in both directions available
                if (previousDayResult.DayCount == 0 && nextDayResult.DayCount == 0)
                {
                    continue;
                }

                // It is only a long weekend if it has more days as a weekend.
                if (previousDayResult.DayCount + nextDayResult.DayCount < this._weekendDays.Count())
                {
                    continue;
                }

                // All bridge days used
                if (previousDayResult.BridgeDayCount + nextDayResult.BridgeDayCount > availableBridgeDays)
                {
                    #region Time Windows Before Holiday

                    var startDate1 = holiday.Date.AddDays(-previousDayResult.DayCount);
                    var endDate1 = holiday.Date;

                    AddUniqueLongWeekend(new LongWeekendReport
                    {
                        NeedBridgeDay = previousDayResult.BridgeDayRequired,
                        BridgeDays = previousDayResult.BridgeDays,
                        StartDate = startDate1,
                        EndDate = endDate1
                    });

                    #endregion

                    #region Time Windows After Holiday

                    var startDate2 = holiday.Date;
                    var endDate2 = holiday.Date.AddDays(nextDayResult.DayCount);

                    AddUniqueLongWeekend(new LongWeekendReport
                    {
                        NeedBridgeDay = nextDayResult.BridgeDayRequired,
                        BridgeDays = nextDayResult.BridgeDays,
                        StartDate = startDate2,
                        EndDate = endDate2
                    });

                    #endregion

                    continue;
                }
                else
                {
                    // Use both areas after and before the holiday and create a time window

                    var startDate = holiday.Date.AddDays(-previousDayResult.DayCount);
                    var endDate = holiday.Date.AddDays(nextDayResult.DayCount);

                    AddUniqueLongWeekend(new LongWeekendReport
                    {
                        NeedBridgeDay = previousDayResult.BridgeDayRequired || nextDayResult.BridgeDayRequired,
                        BridgeDays = [.. previousDayResult.BridgeDays.Concat(nextDayResult.BridgeDays)],
                        StartDate = startDate,
                        EndDate = endDate
                    });
                }
            }

            return items;
        }

        private FollowingDayReport AnalyzeFollowingDays(
            DateOnly startDate,
            DateSearchDirection dateSearchDirection,
            int allowedBridgeDays)
        {
            var availableBridgeDays = allowedBridgeDays;

            var multiplier = 1;
            if (dateSearchDirection == DateSearchDirection.Backward)
            {
                multiplier = -1;
            }

            var calculationDate = startDate.AddDays(1 * multiplier);
            var count = 0;
            var bridgeDayCount = 0;
            var bridgeDayRequired = false;
            var bridgeDays = new List<DateOnly>();
            var checkNextDay = true;

            while (checkNextDay)
            {
                if (this.IsHolidayOrWeekend(calculationDate))
                {
                    calculationDate = calculationDate.AddDays(1 * multiplier);
                    count++;
                    continue;
                }

                var bridgeReport = this.CanJumpWithBridgeDaysToDayOff(calculationDate, dateSearchDirection, availableBridgeDays);
                if (bridgeReport.IsDayOffPossible)
                {
                    bridgeDayRequired = true;
                    bridgeDays.AddRange(bridgeReport.BridgeDays);
                    count += bridgeReport.BridgeDayCount;
                    availableBridgeDays -= bridgeReport.BridgeDayCount;
                    bridgeDayCount += bridgeReport.BridgeDayCount;

                    calculationDate = calculationDate.AddDays(bridgeReport.BridgeDayCount * multiplier);
                    continue;
                }

                checkNextDay = false;
            }

            return new FollowingDayReport
            {
                BridgeDayRequired = bridgeDayRequired,
                BridgeDays = [.. bridgeDays],
                BridgeDayCount = bridgeDayCount,
                DayCount = count
            };
        }

        private DayOffReport CanJumpWithBridgeDaysToDayOff(
            DateOnly startDate,
            DateSearchDirection dateSearchDirection,
            int bridgeDayCount)
        {
            var multiplier = dateSearchDirection == DateSearchDirection.Forward ? 1 : -1;

            for (var bridgeDayIndex = 1; bridgeDayIndex <= bridgeDayCount; bridgeDayIndex++)
            {
                var addDays = bridgeDayIndex * multiplier;

                var isDayOff = this.IsHolidayOrWeekend(startDate.AddDays(addDays));
                if (isDayOff)
                {
                    var bridgeDays = new List<DateOnly>();
                    for (var i = 0; i < bridgeDayIndex; i++)
                    {
                        bridgeDays.Add(startDate.AddDays(i * multiplier));
                    }

                    return new DayOffReport
                    {
                        IsDayOffPossible = true,
                        BridgeDays = [.. bridgeDays],
                        BridgeDayCount = bridgeDayIndex
                    };
                }
            }

            return new DayOffReport
            {
                IsDayOffPossible = false
            };
        }

        private bool IsHolidayOrWeekend(DateOnly givenDate)
        {
            if (this._weekendDays.Contains(givenDate.DayOfWeek))
            {
                return true;
            }

            return this._holidayRecords.Any(o => o.Date.Equals(givenDate));
        }
    }
}
