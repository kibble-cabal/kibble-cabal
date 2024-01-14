@tool
class_name ModifiedStyleBox extends StyleBox

## [Dictionary[[StringName], [Variant]] Key/pairs of [member base_stylebox]'s properties and modified values
@export var modifications: Dictionary:
	set(value):
		modifications = value
		emit_changed()

@export var base_stylebox: StyleBox:
	set(value):
		base_stylebox = value
		if base_stylebox: _connect(base_stylebox)
		emit_changed()

	
@export_group("Add Item Form")

@export var property_name: StringName
@export_enum("SelectAType", "Float", "Int", "Color", "Vector2", "Rect2", "Bool") var property_type: String = "SelectAType"
@export var add_item: bool = false:
	set(value):
		match property_type:
			"Float": modifications[property_name] = 0.0
			"Int": modifications[property_name] = 0
			"Color": modifications[property_name] = Color.WHITE
			"Vector2": modifications[property_name] = Vector2.ZERO
			"Rect2": modifications[property_name] = Rect2()
			"Bool": modifications[property_name] = true
		property_name = &""
		property_type = "SelectAType"
		emit_changed()


func _draw(to_canvas_item: RID, rect: Rect2) -> void:
	var modified_stylebox = _get_modified_stylebox()
	if modified_stylebox:
		modified_stylebox.draw(to_canvas_item, rect)


func _connect(stylebox: StyleBox) -> void:
	Sig.try_connect(stylebox.changed, _on_stylebox_changed)


func _get_modified_stylebox() -> StyleBox:
	if not base_stylebox: return null
	var modified_stylebox = base_stylebox.duplicate(true)
	for property in modifications.keys():
		if property in base_stylebox:
			modified_stylebox.set(property, modifications[property])
	return modified_stylebox


func _on_stylebox_changed() -> void:
	if base_stylebox:
		content_margin_bottom = base_stylebox.content_margin_bottom
		content_margin_left = base_stylebox.content_margin_left
		content_margin_right = base_stylebox.content_margin_right
		content_margin_top = base_stylebox.content_margin_top
