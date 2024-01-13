@tool
class_name MultiPassStyleBox extends StyleBox

@export var passes: Array[StyleBox] = []:
	set(value):
		passes = value
		passes.map(_connect)
		emit_changed()


func _draw(to_canvas_item: RID, rect: Rect2) -> void:
	for stylebox in passes:
		if stylebox: stylebox.draw(to_canvas_item, rect)


func _connect(stylebox: StyleBox) -> void:
	Sig.try_connect(stylebox.changed, emit_changed)
