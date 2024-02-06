extends PanelContainer


@export var building: BuildingResource

@onready var roof_list := %RoofItemList as ItemList
@onready var history := BuildModeState.get_history()


func _ready() -> void:
	ItemDB.item_registered.connect(func(_item): update())
	ItemDB.item_unregistered.connect(func(_item): update())
	update()


func _on_roof_item_list_item_selected(index: int) -> void:
	var item_id = roof_list.get_item_metadata(index)
	if building and history: history.add(
		building,
		"Set Roof",
		building.set.bind(&"roof_id", item_id),
		building.set.bind(&"roof_id", building.roof_id)
	)


func update() -> void:
	update_list(roof_list, get_roofs())
	
	if building:
		update_selection(roof_list, building.roof_id)


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


func get_roofs() -> Array[ItemResource]:
	return get_items("roof")
