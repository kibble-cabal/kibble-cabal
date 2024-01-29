extends Object


const Items = [
	preload("item/resources/basic_wood_floor.tres"),
	preload("item/resources/striped_wall_with_trim.tres"),
	preload("item/resources/basic_siding.tres"),
]


func _init() -> void:
	Items.map(ItemDB.register)
