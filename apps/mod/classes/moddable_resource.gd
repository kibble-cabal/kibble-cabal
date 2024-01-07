class_name ModdableResource extends Resource


## [Dictionary][[String], [Resource]] Stores arbitrary child resources
@export var subresources: Dictionary


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


func lua_fields() -> Array[String]:
	return [
		"subresources",
		"get_subresource",
		"add_subresource",
		"remove_subresource",
		"find_subresource_key"
	]
