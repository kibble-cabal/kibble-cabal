extends PanelContainer


@export var room: RoomResource

@onready var floor_list := %FloorItemList as ItemList
@onready var interior_list := %InteriorItemList as ItemList
@onready var exterior_list := %ExteriorItemList as ItemList


func _ready() -> void:
	ItemDB.item_registered.connect(func(_item): update())
	ItemDB.item_unregistered.connect(func(_item): update())
	update()


func _on_floor_item_list_item_selected(index: int) -> void:
	var item_id = floor_list.get_item_metadata(index)
	if room: room.floor_id = item_id


func _on_interior_item_list_item_selected(index: int) -> void:
	var item_id = interior_list.get_item_metadata(index)
	if room: room.interior_id = item_id


func _on_exterior_item_list_item_selected(index: int) -> void:
	var item_id = exterior_list.get_item_metadata(index)
	if room: room.exterior_id = item_id


func update() -> void:
	update_list(floor_list, get_floors())
	update_list(interior_list, get_walls())
	update_list(exterior_list, get_walls())
	
	if room:
		update_selection(floor_list, room.floor_id)
		update_selection(interior_list, room.interior_id)
		update_selection(exterior_list, room.exterior_id)


func update_selection(list: ItemList, value: StringName) -> void:
	for i in list.item_count:
		var item_id: StringName = list.get_item_metadata(i)
		if item_id == value:
			list.select(i)
		


func update_list(list: ItemList, items: Array[ItemResource]) -> void:
	list.clear()
	list.add_item("None")
	list.set_item_metadata(0, &"")
	for item in items:
		list.add_item(item.display_name, item.icon)
		list.set_item_metadata(list.item_count - 1, item.id)


func get_items(category: String) -> Array[ItemResource]:
	return ItemDB.registered_items.filter(
		func(item: ItemResource) -> bool: return item.id.begins_with("build/{0}/".format([category]))
	)


func get_floors() -> Array[ItemResource]:
	return get_items("floor")


func get_walls() -> Array[ItemResource]:
	return get_items("wall")
