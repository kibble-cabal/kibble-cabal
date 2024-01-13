class_name MusicLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetMusicDB", func(): return MusicDB)
	lua.push_variant("GetMusicSystem", func(): return MusicSystem)


func expose_hooks(lua: LuaAPI) -> void:
	MusicDB.music_registered.connect(
		func(music: MusicResource) -> void:
			lua.call_function("OnMusicRegistered", [music])
	)
	MusicDB.music_unregistered.connect(
		func(music: MusicResource) -> void:
			lua.call_function("OnMusicUnregistered", [music])
	)
