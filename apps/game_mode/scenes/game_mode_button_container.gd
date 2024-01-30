extends CircleContainer

const ButtonScene := preload("game_mode_button.tscn")

@onready var background := $ColorRect as ColorRect

var button_nodes := {}


func _ready() -> void:
	update()
	background.pivot_offset = background.size / 2
	background.rotation_degrees = -30
	
	Sig.try_connect(GameModeDB.game_mode_registered, func(_mode): update())
	Sig.try_connect(GameModeDB.game_mode_unregistered, func(_mode): update())


func update_focus() -> void:
	if GameModeSystem.current_mode and GameModeSystem.current_mode in button_nodes:
		button_nodes[GameModeSystem.current_mode].grab_focus()


func update() -> void:
	for game_mode: GameModeResource in GameModeDB.registered_game_modes:
		if not game_mode in button_nodes and game_mode.ui_menu_index >= 0:
			var scene := ButtonScene.instantiate()
			scene.game_mode = game_mode
			scene.size_flags_horizontal = Control.SIZE_SHRINK_BEGIN
			scene.size_flags_vertical = Control.SIZE_SHRINK_CENTER
			button_nodes[game_mode] = scene
			add_child(scene)
	
	# Remove outdated buttons, organize buttons
	for game_mode: GameModeResource in button_nodes.keys():
		if not game_mode in GameModeDB.registered_game_modes:
			button_nodes[game_mode].queue_free()
			button_nodes.erase(game_mode)
		else:
			move_child(button_nodes[game_mode], game_mode.ui_menu_index)


func _sort_children() -> void:
	super()
	# Rotate children
	var children := get_controlled_children()
	if children.size() > 0:
		var total_degrees := 30
		for i in range(children.size()):
			children[i].rotation_degrees = i * total_degrees / children.size() - total_degrees / 2
