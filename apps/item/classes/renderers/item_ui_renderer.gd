class_name ItemUiRenderer extends Renderer


@warning_ignore("unused_variable")
static func render(node: Node, item: ItemResource, config := {}) -> TextureRect:
	var texture := TextureRect.new()
	texture.texture = item.icon
	texture.tooltip_text = item.display_name
	texture.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR
	return texture
