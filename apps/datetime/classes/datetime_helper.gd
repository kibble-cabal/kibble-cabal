class_name DatetimeHelper

const SEASONS_IN_YEAR = 4
const WEEKS_IN_SEASON = 28
const DAYS_IN_WEEK = 7
const HOURS_IN_DAY = 24
const MINUTES_IN_HOUR = 60

const MINUTES_IN_DAY = MINUTES_IN_HOUR * HOURS_IN_DAY
const MINUTES_IN_WEEK = MINUTES_IN_DAY * DAYS_IN_WEEK
const MINUTES_IN_SEASON = MINUTES_IN_WEEK * WEEKS_IN_SEASON
const MINUTES_IN_YEAR = MINUTES_IN_SEASON * SEASONS_IN_YEAR

enum Season {
	SPRING = 1,
	SUMMER = 2,
	FALL = 3,
	WINTER = 4
}

const SeasonShort = {
	[Season.SPRING]: "Sp",
	[Season.SUMMER]: "Su",
	[Season.FALL]: "F",
	[Season.WINTER]: "W"
}

enum Day {
	MONDAY = 1,
	TUESDAY = 2,
	WEDNESDAY = 3,
	THURSDAY = 4,
	FRIDAY = 5,
	SATURDAY = 6,
	SUNDAY = 7
}

const DayShort = {
	[Day.MONDAY]: "Mon",
	[Day.TUESDAY]: "Tue",
	[Day.WEDNESDAY]: "Wed",
	[Day.THURSDAY]: "Thu",
	[Day.FRIDAY]: "Fri",
	[Day.SATURDAY]: "Sat",
	[Day.SUNDAY]: "Sun",
}


static func get_year(time: int) -> int:
	return _floor(time, MINUTES_IN_YEAR)


static func get_season(time: int) -> int:
	return _floor(time % MINUTES_IN_YEAR, MINUTES_IN_SEASON)


static func get_week(time: int) -> int:
	return _floor(time % MINUTES_IN_SEASON, MINUTES_IN_WEEK)


static func get_week_of_year(time: int) -> int:
	return _floor(time % MINUTES_IN_YEAR, MINUTES_IN_WEEK)


static func get_day(time: int) -> int:
	return _floor(time % MINUTES_IN_WEEK, MINUTES_IN_DAY)


static func get_date(time: int) -> int:
	return _floor(time % MINUTES_IN_SEASON, MINUTES_IN_DAY)


static func get_hour(time: int) -> int:
	return _floor(time % MINUTES_IN_DAY, MINUTES_IN_HOUR)


static func get_minute(time: int) -> int:
	return time % MINUTES_IN_HOUR


static func get_dict(time: int) -> Dictionary:
	return {
		year = get_year(time),
		season = get_season(time),
		week = get_week(time),
		date = get_date(time),
		day = get_day(time),
		hour = get_hour(time),
		minute = get_minute(time)
	}


static func from_dict(time: Dictionary) -> int:
	return (
		time.year * MINUTES_IN_YEAR
		+ time.season * MINUTES_IN_SEASON
		+ time.week * MINUTES_IN_WEEK
		+ time.day * MINUTES_IN_DAY
		+ time.hour + MINUTES_IN_HOUR
		+ time.minute
	)


## Provide a format string that has any of the following:
## [br] - [code]year[/code] – Year (e.g. [code]2[/code])
## [br] - [code]season[/code] – Season (string) (e.g. [code]Spring[/code])
## [br] - [code]SeasonShort[/code] – Season (short) (e.g. [code]F[/code], [code]Sp[/code])
## [br] - [code]season_number[/code] – Season (number) (e.g. [code]1[/code])
## [br] - [code]week[/code] – Week of season (number) (e.g. [code]2[/code])
## [br] - [code]week_of_year[/code] – Week of year (number) (e.g. [code]6[/code])
## [br] - [code]date[/code] – Date of season (number) (e.g. [code]14[/code])
## [br] - [code]day[/code] – Day of week (string) (e.g. [code]Sunday[/code])
## [br] - [code]DayShort[/code] – Day of week (shortened string) (e.g. [code]Sun[/code])
## [br] - [code]day_number[/code] – Day of week (number) (e.g. [code]7[/code])
## [br] - [code]hour[/code] – Hour (number) (e.g. [code]2[/code])
## [br] - [code]hour_pad[/code] – Hour (number with padded zero) (e.g. [code]02[/code])
## [br] - [code]minute[/code] – Minute (number) (e.g. [code]45[/code])
## [br] - [code]minute_pad[/code] – Minute (number with padded zero) (e.g. [code]05[/code])
## [br][br][b]Example format string[/b][br]
## [code]"{hour_pad}:{minute_pad} on {DayShort}, {season} {date}, year {year}"[/code]
## becomes [code]"01:45 on Mon, Spring 1, Year 2"[/code]
static func format(time: int, string: String = "") -> String:
	var dict := get_dict(time)
	return string.format({
		year = dict.year,
		season = Season.find_key(dict.season).to_pascal_case(),
		SeasonShort = SeasonShort[dict.season],
		season_number = dict.season,
		week = dict.week,
		week_of_year = get_week_of_year(time),
		date = dict.date,
		day = Day.find_key(dict.day).to_pascal_case(),
		DayShort = DayShort[dict.day],
		day_number = dict.day,
		hour = dict.hour,
		hour_pad = str(dict.hour).pad_zeros(2),
		minute = dict.minute,
		minute_pad = str(dict.minute).pad_zeros(2)
	})


static func _floor(a: int, b: int) -> int:
	return floori(float(a) / float(b))


func lua_fields() -> Array[String]:
	return [
		"SEASONS_IN_YEAR",
		"WEEKS_IN_SEASON",
		"DAYS_IN_WEEK",
		"HOURS_IN_DAY",
		"MINUTES_IN_HOUR",
		"MINUTES_IN_DAY",
		"MINUTES_IN_WEEK",
		"MINUTES_IN_SEASON",
		"MINUTES_IN_YEAR",
		"Day",
		"DayShort",
		"Season",
		"SeasonShort",
		"get_year",
		"get_season",
		"get_week",
		"get_week_of_year",
		"get_date",
		"get_day",
		"get_hour",
		"get_minute",
		"get_dict",
		"from_dict",
		"formatted"
	]
