class_name SoundEffectLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSoundEffectDB", func(): return SoundEffectDB)
	lua.push_variant("GetSoundManager", func(): return SoundManager)
	lua.push_variant("SoundHelper", Sound.new())


func expose_hooks(lua: LuaAPI) -> void:
	SoundEffectDB.sound_effect_registered.connect(
		func(sound: AudioStream) -> void:
			lua.call_function("OnSoundEffectRegistered", [sound])
	)
	SoundEffectDB.sound_effect_unregistered.connect(
		func(sound: AudioStream) -> void:
			lua.call_function("OnSoundEffectUnregistered", [sound])
	)
