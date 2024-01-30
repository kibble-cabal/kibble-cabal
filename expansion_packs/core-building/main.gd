extends Object

const GameModes = [
	preload("game_mode/resources/build_mode.tres")
]

const Items = [
	preload("item/resources/basic_wood_floor.tres"),
	preload("item/resources/striped_wall_with_trim.tres"),
	preload("item/resources/basic_siding.tres"),
]


func _init() -> void:
	GameModes.map(GameModeDB.register)
	Items.map(ItemDB.register)
