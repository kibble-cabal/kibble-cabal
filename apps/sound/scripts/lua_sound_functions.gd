class_name LuaSoundFunctions extends Object

## NOTE: Unfinished class.

class LuaSoundPlayer:
	var sound: AudioStream
	func _init(sound_path: String) -> void:
		self.sound = ResourceLoader.load(sound_path, "AudioStream")


static func setup(lua: LuaAPI) -> void:
	lua.expose_constructor("SoundPlayer", LuaSoundPlayer)
