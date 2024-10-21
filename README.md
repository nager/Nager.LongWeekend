# Nager.LongWeekend

Nager.LongWeekend is an open-source project designed to calculate potential long weekends based on public holidays.
By providing a list of holidays, it determines where extended weekends are possible.

## nuget

The package is available on [nuget](https://www.nuget.org/packages/Nager.LongWeekend)
```
PM> install-package Nager.LongWeekend
```

## Example

```cs
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

var longWeekendCalculator = new LongWeekendCalculator(holidays, weekendDays);
var longWeekends = longWeekendCalculator.Calculate(availableBridgeDays);
```