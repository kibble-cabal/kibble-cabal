class_name SoundEffectLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSoundEffectDB", func(): return SoundEffectDB)
	lua.push_variant("GetSoundManager", func(): return SoundManager)
	lua.push_variant("SoundHelper", Sound.new())
