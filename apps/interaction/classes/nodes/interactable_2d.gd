@tool
extends Area2D

## Forked from [url=https://github.com/MASSHUU12/godot-interaction-system]Godot Interaction System[/url]

## Class used to create interactive objects in 2D space.
class_name Interactable2D

## Emitted when an [Interactor2D] starts looking at object.
signal focused(interactor: Interactor2D)
## Emitted when an [Interactor2D] stops looking at object.
signal unfocused(interactor: Interactor2D)
## Emitted when an [Interactor2D] interacts with an object.
signal interacted(interactor: Interactor2D)
## Emitted when an [Interactable2D] is the closest to the [Interactor2D].
signal closest(interactor: Interactor2D)
## Emitted when an [Interactable2D] is no longer the closest one to the [Interactor2D].
signal not_closest(interactor: Interactor2D)


var mouse_is_over: bool = false


func _init() -> void:
	input_event.connect(_on_input_event)
	mouse_entered.connect(func(): mouse_is_over = true)
	mouse_exited.connect(func(): mouse_is_over = false)


func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.is_pressed() and mouse_is_over:
		get_viewport().set_input_as_handled()


func _on_input_event(_viewport, event: InputEvent, _shape_index: int) -> void:
	if event is InputEventMouseButton and event.is_pressed():
		for node in get_overlapping_bodies() + get_overlapping_areas():
			if node.get_parent() is PlayerRoot:
				interacted.emit(node.get_parent() as Interactor2D)
