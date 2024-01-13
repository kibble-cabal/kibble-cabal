class_name MusicResource extends ModdableResource

class Song extends ModdableResource:
	@export var key: String
	@export var value: AudioStream

@export var id: String
@export var songs: Array[Song] = []

## This script should have a method ([member selector_method]) that picks a [Song] based on the current game state.
@export var selector_script: Script

## Should have the following signature:
## [br][code]func(save: SaveResource) -> AudioStream[/code]
@export var selector_method: StringName


func get_song() -> AudioStream:
	if selector_script and selector_method:
		var selector = selector_script.new()
		if selector.has_method(selector_method):
			return selector[selector_method].call(SaveSystem.current_save)
	if songs.size():
		return songs[0].value
	return null
