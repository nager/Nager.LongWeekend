using FluentAssertions;
using Nager.LongWeekend;
using Nager.LongWeekend.Models;

namespace Nager.Date.UnitTest.Common
{
    [TestClass]
    public class LongWeekendTest
    {
        [TestMethod]
        public void Calculate_FromFridayToMonday_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │ WE │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │ 15 │
            // ├────┼────┼────┼────┼────┼────┼────┤
            // │    │ PH │ WE │ WE │ PH │    │    │
            // └────┴────┴────┴────┴────┴────┴────┘
            //        └──────────────┘
            //             4 days

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 10),
                    Name = "Holiday Friday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 13),
                    Name = "Holiday Monday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 1;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(1, longWeekends.Count());

            var firstLongWeekend = longWeekends.First();

            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 10),
                EndDate = new DateOnly(2020, 01, 13),
                BridgeDays = [],
                NeedBridgeDay = false
            });
        }

        [TestMethod]
        public void Calculate_VeryLongWeekendWithoutBridgeDay_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │ WE │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │ 15 │
            // ├────┼────┼────┼────┼────┼────┼────┤
            // │    │ PH │ WE │ WE │ PH │ PH │    │
            // └────┴────┴────┴────┴────┴────┴────┘
            //        └────────────────────┘
            //                5 days

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 10),
                    Name = "Holiday Friday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 13),
                    Name = "Holiday Monday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 14),
                    Name = "Holiday Tuesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 1;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(1, longWeekends.Count());

            var firstLongWeekend = longWeekends.First();

            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 10),
                EndDate = new DateOnly(2020, 01, 14),
                BridgeDays = [],
                NeedBridgeDay = false
            });
        }

        [TestMethod]
        public void Calculate_VeryLongWeekendWithOneBridgeDay_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │
            // ├────┼────┼────┼────┼────┼────┤
            // │    │ PH │ WE │ WE │ BD │ PH │
            // └────┴────┴────┴────┴────┴────┘
            //        └────────────────────┘
            //                 5 days

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 10),
                    Name = "Holiday Friday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 14),
                    Name = "Holiday Tuesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 1;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(1, longWeekends.Count());

            var firstLongWeekend = longWeekends.First();

            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 10),
                EndDate = new DateOnly(2020, 01, 14),
                BridgeDays = [new DateOnly(2020, 01, 13)],
                NeedBridgeDay = true
            });
        }

        [TestMethod]
        public void Calculate_VeryLongWeekendWithThreeBridgeDays_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │ WE │ TU │ FR │ SA │ SU │ MO │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │ 15 │ 16 │ 17 │ 18 │ 19 │ 20 │
            // ├────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┤
            // │    │ PH │ WE │ WE │ BD │ PH │ BD │ BD │ BD │ WE │ WE │    │
            // └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
            //        └────────────────────┘
            //                 5 days
            //                           └──────────────────────────┘
            //                                      6 days

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 10),
                    Name = "Holiday Friday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 14),
                    Name = "Holiday Tuesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 3;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays).ToArray();

            Assert.AreEqual(2, longWeekends.Count());

            var firstLongWeekend = longWeekends[0];
            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 10),
                EndDate = new DateOnly(2020, 01, 14),
                BridgeDays = [new DateOnly(2020, 01, 13)],
                NeedBridgeDay = true
            });

            var secondLongWeekend = longWeekends[1];
            secondLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 14),
                EndDate = new DateOnly(2020, 01, 19),
                BridgeDays = [new DateOnly(2020, 01, 15), new DateOnly(2020, 01, 16), new DateOnly(2020, 01, 17)],
                NeedBridgeDay = true
            });
        }

        [TestMethod]
        public void Calculate_LongWeekendWithTwoBridgeDays_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │
            // ├────┼────┼────┼────┼────┼────┤
            // │ PH │ BD │ WE │ WE │ BD │ PH │
            // └────┴────┴────┴────┴────┴────┘
            //   └─────────────────────────┘
            //     6 days (2 bridge days)

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 9),
                    Name = "Holiday Thursday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 14),
                    Name = "Holiday Tuesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 2;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(1, longWeekends.Count());

            var firstLongWeekend = longWeekends.First();

            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 9),
                EndDate = new DateOnly(2020, 01, 14),
                BridgeDays = [new DateOnly(2020, 01, 10), new DateOnly(2020, 01, 13)],
                NeedBridgeDay = true
            });
        }

        [TestMethod]
        public void Calculate_TwoLongWeekendsWithOneBridgeDay_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │
            // ├────┼────┼────┼────┼────┼────┤
            // │ PH │ BD │ WE │ WE │ BD │ PH │
            // └────┴────┴────┴────┴────┴────┘
            //   └───────────────┘
            //   4 days (1 bridge days)
            //              └───────────────┘
            //               4 days (1 bridge days)

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 9),
                    Name = "Holiday Thursday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 14),
                    Name = "Holiday Tuesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 1;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(2, longWeekends.Count());

            var firstLongWeekend = longWeekends.First();

            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 9),
                EndDate = new DateOnly(2020, 01, 12),
                BridgeDays = [new DateOnly(2020, 01, 10)],
                NeedBridgeDay = true
            });

            var secondLongWeekend = longWeekends.Last();

            secondLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 11),
                EndDate = new DateOnly(2020, 01, 14),
                BridgeDays = [new DateOnly(2020, 01, 13)],
                NeedBridgeDay = true
            });
        }

        [TestMethod]
        public void Calculate_FourLongWeekendsWithTwoBridgeDays_Successful1()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
            // │ SA │ SU │ MO │ TU │ WE │ TH │ FR │ SA │ SU │ MO │ TU │ WE │ TH │ FR │ SA │ SU │
            // │ 04 │ 05 │ 06 │ 07 │ 08 │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │ 15 │ 16 │ 17 │ 18 │ 19 │
            // ├────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┼────┤
            // │ WE │ WE │ BD │ BD │ PH │ BD │ BD │ WE │ WE │ BD │ BD │ PH │ BD │ BD │ WE │ WE │
            // └────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
            //   └────────────────────┘
            //   5 days (2 bridge days)
            //                       └────────────────────┘
            //                       5 days (2 bridge days)
            //                                      └────────────────────┘
            //                                      5 days (2 bridge days)
            //                                                          └────────────────────┘
            //                                                          5 days (2 bridge days)

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 8),
                    Name = "Holiday Wednesday"
                },
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 15),
                    Name = "Holiday Wednesday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 2;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays).ToArray();

            Assert.AreEqual(4, longWeekends.Length);

            var firstLongWeekend = longWeekends[0];
            firstLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 04),
                EndDate = new DateOnly(2020, 01, 08),
                BridgeDays = [new DateOnly(2020, 01, 06), new DateOnly(2020, 01, 07)],
                NeedBridgeDay = true
            });

            var secondLongWeekend = longWeekends[1];
            secondLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 08),
                EndDate = new DateOnly(2020, 01, 12),
                BridgeDays = [new DateOnly(2020, 01, 09), new DateOnly(2020, 01, 10)],
                NeedBridgeDay = true
            });

            var thirdLongWeekend = longWeekends[2];
            thirdLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 11),
                EndDate = new DateOnly(2020, 01, 15),
                BridgeDays = [new DateOnly(2020, 01, 13), new DateOnly(2020, 01, 14)],
                NeedBridgeDay = true
            });

            var fourthLongWeekend = longWeekends[3];
            fourthLongWeekend.Should().BeEquivalentTo(new LongWeekendReport
            {
                StartDate = new DateOnly(2020, 01, 15),
                EndDate = new DateOnly(2020, 01, 19),
                BridgeDays = [new DateOnly(2020, 01, 16), new DateOnly(2020, 01, 17)],
                NeedBridgeDay = true
            });
        }

        [TestMethod]
        public void Calculate_NoLongWeekendPossible_Successful()
        {
            // Legend
            // PH = PublicHoliday
            // WE = Weekend
            // BD = BridgeDay
            // ┌────┬────┬────┬────┬────┬────┐
            // │ TH │ FR │ SA │ SU │ MO │ TU │
            // │ 09 │ 10 │ 11 │ 12 │ 13 │ 14 │
            // ├────┼────┼────┼────┼────┼────┤
            // │    │    │ WE │ WE │    │    │
            // │    │    │ PH │    │    │    │
            // └────┴────┴────┴────┴────┴────┘
            //             └────┘
            // Public Holiday falls on a weekend

            var holidays = new HolidayRecord[]
            {
                new HolidayRecord
                {
                    Date = new DateOnly(2020, 01, 11),
                    Name = "Holiday Saturday"
                }
            };

            var weekendDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var availableBridgeDays = 1;

            ILongWeekendCalculator longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
            var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);

            Assert.AreEqual(0, longWeekends.Count());
        }
    }
}
