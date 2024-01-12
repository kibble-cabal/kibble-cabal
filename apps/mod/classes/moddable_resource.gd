class_name ModdableResource extends Resource

signal subresources_changed


## [Dictionary][[String], [Resource]] Stores arbitrary child resources
@export var subresources: Dictionary:
	set(value):
		subresources = value
		subresources_changed.emit()


func get_subresource(key: String) -> Resource:
	return subresources.get(key, null) as Resource


func add_subresource(key: String, value: Resource) -> void:
	subresources[key] = value


func remove_subresource(key: String) -> void:
	if subresources.has(key):
		subresources.erase(key)


func find_subresource_key(by: Callable) -> String:
	for key in subresources.keys():
		if by.call(subresources[key], key) == true:
			return key
	return ""


func lua_fields() -> Array:
	return [
		"subresources",
		"get_subresource",
		"add_subresource",
		"remove_subresource",
		"find_subresource_key"
	]


## Emits the [signal Resource.changed] signal on this resource when the provided child resource is changed
func _connect_subresource(subresource: Resource) -> void:
	if subresource is Resource and not subresource.changed.is_connected(emit_changed):
		subresource.changed.connect(emit_changed)
