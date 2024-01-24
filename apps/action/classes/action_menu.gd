@tool
class_name ActionMenu extends CircleContainer

signal opening
signal opened
signal closing
signal closed

@export var menu_identifier: StringName
@export var additional_actions: Array[ActionMenuItem] = []
@export var close_on_select: bool = true

var nodes := {}


func _enter_tree() -> void:
	for action in get_all_actions():
		nodes[action] = action.render()
		Sig.try_connect(nodes[action].pressed, _on_item_pressed)
		add_child(nodes[action])
	visible = false


func get_all_actions() -> Array[ActionMenuItem]:
	var items := additional_actions.duplicate()
	items.append_array(ActionDB.find_by_menu(menu_identifier))
	return items


func open(ctx = null) -> void:
	opening.emit()
	_update_items(ctx)
	show()
	opened.emit()


func close() -> void:
	closing.emit()
	hide()
	closed.emit()


func _update_items(ctx = null) -> void:
	for action in get_all_actions():
		if action in nodes and nodes[action] is Button:
			action.update(nodes[action], ctx)
		else:
			nodes[action] = action.render(ctx)
			add_child(nodes[action])
			action.update(nodes[action], ctx)
		Sig.try_connect(nodes[action].pressed, _on_item_pressed)


func _on_item_pressed() -> void:
	if close_on_select: close()


func lua_fields() -> Array:
	return ["open", "close", "get_all_actions", "menu_identifier", "additional_actions", "close_on_select"]
