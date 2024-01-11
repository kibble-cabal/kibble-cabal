class_name SaveResource extends ModdableResource

@export var id: String = ""
@export var settings := SettingsResource.new():
	set(value):
		settings = value
		_connect_subresource(settings)

@export var player := PlayerResource.new():
	set(value):
		player = value
		_connect_subresource(player)

@export var inventory := InventoryResource.new():
	set(value):
		inventory = value
		_connect_subresource(inventory)

## The state of each location instance for this save game
@export var location_states: Array[LocationStateResource] = []

@export var fate := FateResource.new():
	set(value):
		fate = value
		_connect_subresource(fate)

@export var datetime := DatetimeResource.new():
	set(value):
		datetime = value
		_connect_subresource(datetime)

## Datetime that this file was last saved.
@export var last_saved: String

## Time (in seconds) that this file has been opened.
@export var time_played: float = 0


var _save_helper := SaveHelper.new({
	resource = self,
	base_dir = func() -> String: return "user://saves/{0}".format([id]),
	filename = "save",
	ignore_resource_path = true,
	save_on_change = true
})


func _init() -> void:
	_connect_all_subresources()
	subresources_changed.connect(_connect_all_subresources)


func commit_changes() -> void:
	if len(id) == 0: _generate_id()
	self.last_saved = Time.get_datetime_string_from_system()
	_save_helper.commit()


func get_or_create_location_state(location_name: String) -> LocationStateResource:
	for state in location_states:
		if state.location_name == location_name: return state
	var state := LocationStateResource.new()
	state.location_name = location_name
	location_states.append(state)
	return state


func lua_fields() -> Array[String]:
	return super() + ["settings", "player", "inventory", "location_states", "fate", "datetime", "commit_changes"]


func _generate_id() -> void:
	if not DirAccess.dir_exists_absolute(_save_helper.get_dir()): id = "Save_0"
	else:
		var num_directories := DirAccess.get_directories_at(_save_helper.get_dir()).size()
		id = "Save_{0}".format([num_directories])
	DirAccess.make_dir_recursive_absolute(_save_helper.get_dir())


## Performs [method _connect_subresource] for all child resources
func _connect_all_subresources() -> void:
	for subresource in [settings, player, inventory, fate, datetime] + location_states + subresources.values():
		_connect_subresource(subresource)
