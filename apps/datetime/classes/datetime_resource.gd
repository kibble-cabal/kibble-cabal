class_name DatetimeResource extends ModdableResource

const SEASONS_IN_YEAR = 4
const WEEKS_IN_SEASON = 28
const DAYS_IN_WEEK = 7
const HOURS_IN_DAY = 24
const MINUTES_IN_HOUR = 60

const MINUTES_IN_DAY = MINUTES_IN_HOUR * HOURS_IN_DAY
const MINUTES_IN_WEEK = MINUTES_IN_DAY * DAYS_IN_WEEK
const MINUTES_IN_SEASON = MINUTES_IN_WEEK * WEEKS_IN_SEASON
const MINUTES_IN_YEAR = MINUTES_IN_SEASON * SEASONS_IN_YEAR

## How many real-world seconds is equal to one in-game minute
const TIME_SPEED: float = 10.0

## The number of in-game minutes that have passed.
@export var current_time: int = 0

## How quickly time passes in game.
## [br]See [method DatetimeSystem.get_wait_time]
@export var time_speed_multiplier: float = 1.0


func get_year() -> int:
	return _floor(current_time, MINUTES_IN_YEAR)


func get_season() -> int:
	return _floor(current_time % MINUTES_IN_YEAR, MINUTES_IN_SEASON)


func get_week() -> int:
	return _floor(current_time % MINUTES_IN_SEASON, MINUTES_IN_WEEK)


func get_day_of_week() -> int:
	return _floor(current_time % MINUTES_IN_WEEK, MINUTES_IN_DAY)


func get_day_of_season() -> int:
	return _floor(current_time % MINUTES_IN_SEASON, MINUTES_IN_DAY)


func get_hour() -> int:
	return _floor(current_time % MINUTES_IN_DAY, MINUTES_IN_HOUR)


func get_minute() -> int:
	return current_time % MINUTES_IN_HOUR


func _floor(a: int, b: int) -> int:
	return floori(float(a) / float(b))
