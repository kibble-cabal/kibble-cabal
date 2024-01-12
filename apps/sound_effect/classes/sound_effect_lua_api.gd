class_name SoundEffectLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetSoundEffectDB", func(): return SoundEffectDB)
