@tool
extends EditorPlugin


func _enter_tree() -> void:
	add_autoload_singleton("UIAnimManager", "autoload/ui_anim_manager.gd")


func _exit_tree() -> void:
	remove_autoload_singleton("UIAnimManager")
