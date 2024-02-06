class_name BuildModeState extends GameModeState


var history := History.new()


func _init() -> void:
	history.ui_node = make_history_ui()


func _input(event: InputEvent) -> void:
	if history and event.is_action_pressed("ui_undo"):
		history.undo()
	
	if history and event.is_action_pressed("ui_redo"):
		history.redo()


func make_history_ui() -> Control:
	var ui_root := UIConfig.get_game_mode_ui_root()
	var node := VBoxContainer.new()
	var on_child_change := func(_child = null) -> void:
		node.set_anchors_and_offsets_preset(Control.PRESET_BOTTOM_RIGHT)
	node.child_order_changed.connect(on_child_change)
	node.tree_entered.connect(on_child_change)
	ui_root.add_child(node)
	return node


static func get_history() -> History:
	if GameModeSystem.current_state is BuildModeState:
		return (GameModeSystem.current_state as BuildModeState).history
	return null
