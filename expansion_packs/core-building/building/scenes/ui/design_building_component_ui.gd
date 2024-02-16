extends ItemList

signal selected(id: StringName)

@export var category: String

@onready var history := BuildModeState.get_history()

var selected_item: StringName


func _ready() -> void:
	ItemDB.item_registered.connect(func(_item): update())
	ItemDB.item_unregistered.connect(func(_item): update())
	if history: history.changed.connect(update)
	item_selected.connect(_on_item_selected)
	update()


func _on_item_selected(index: int) -> void:
	selected_item = get_item_metadata(index)
	selected.emit(selected_item)


func select_item(id: StringName) -> void:
	for i in item_count:
		if get_item_metadata(i) == id:
			return select(i)


func update() -> void:
	var items := get_items()
	clear()
	add_item("None")
	set_item_metadata(0, &"")
	for item in items:
		add_item(item.display_name, item.icon)
		set_item_metadata(item_count - 1, item.id)
	if selected_item:
		select_item(selected_item)


func get_items() -> Array[ItemResource]:
	return ItemDB.registered_items.filter(
		func(item: ItemResource) -> bool: return item.id.begins_with("build/{0}/".format([category]))
	)
