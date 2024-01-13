# UIConfig
extends Node

# Sounds

var ButtonSound: AudioStream
var PauseSound: AudioStream
var ResumeSound: AudioStream
var TypingSound: AudioStream

# Scenes

var PauseScene: PackedScene


func get_ui_root() -> Node:
	return get_tree().get_first_node_in_group("ui_root")


func lua_fields() -> Array:
	return [
		"ButtonSound",
		"PauseScene",
		"ResumeSound",
		"TypingSound"
	]
