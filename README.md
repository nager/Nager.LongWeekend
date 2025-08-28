# Nager.LongWeekend

**Nager.LongWeekend** is a lightweight, open-source .NET library that helps you calculate potential long weekends based on public holidays.  
By providing a list of holidays and your defined weekend days, the library determines possible extended weekends — including those that can be created with optional bridge days.  

## ✨ Features  
- Detects long weekends based on given holiday data.  
- Supports custom weekend definitions (not just Saturday/Sunday).  
- Considers bridge days to extend weekends.  
- Simple API with minimal dependencies.  

## 📦 Installation

The package is available on [NuGet](https://www.nuget.org/packages/Nager.LongWeekend)
```powershell
PM> install-package Nager.LongWeekend
```

## 🚀 Usage Examples

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
