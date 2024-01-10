class_name ItemInstanceUiRenderer extends Renderer


@warning_ignore("unused_parameter")
static func render(node: Node, item_instance: ItemInstanceResource, config := {}) -> void:
	var item = item_instance.get_item_resource()
	var texture := TextureRect.new()
	if item:
		texture.texture = item.icon
		texture.tooltip_text = item.display_name
		# pixelart-specific
		texture.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST
		texture.scale = Vector2(4, 4)
	else:
		texture.texture = PlaceholderTexture2D.new()
	node.add_child(texture)
