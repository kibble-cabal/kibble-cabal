class_name DatetimeResource extends ModdableResource

## How many real-world seconds is equal to one in-game minute
const TIME_SPEED: float = 5.0

## The number of in-game minutes that have passed.
@export var current_time: int = 0

## How quickly time passes in game.
## [br]See [method DatetimeSystem.get_wait_time]
@export var time_speed_multiplier: float = 1.0


func get_year() -> int:
	return DatetimeHelper.get_year(current_time)


func get_season() -> int:
	return DatetimeHelper.get_season(current_time)


func get_week() -> int:
	return DatetimeHelper.get_week(current_time)


func get_week_of_year() -> int:
	return DatetimeHelper.get_week_of_year(current_time)


func get_day() -> int:
	return DatetimeHelper.get_day(current_time)


func get_date() -> int:
	return DatetimeHelper.get_date(current_time)


func get_hour() -> int:
	return DatetimeHelper.get_hour(current_time)


func get_minute() -> int:
	return DatetimeHelper.get_minute(current_time)


func lua_fields() -> Array:
	return super() + [
		"current_time",
		"time_speed_multiplier",
		"get_year",
		"get_season",
		"get_week",
		"get_week_of_year",
		"get_date",
		"get_day",
		"get_hour",
		"get_minute"
	]
