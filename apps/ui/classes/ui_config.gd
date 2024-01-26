class_name UIConfig

# Sounds

static var ButtonSound: AudioStream
static var PauseSound: AudioStream
static var ResumeSound: AudioStream
static var TypingSound: AudioStream

# Scenes

static var PauseScene: PackedScene


static func get_ui_root() -> Node:
	return Meta.get_tree().get_first_node_in_group("ui_root")


static func lua_fields() -> Array:
	return [
		"ButtonSound",
		"PauseScene",
		"ResumeSound",
		"TypingSound"
	]
