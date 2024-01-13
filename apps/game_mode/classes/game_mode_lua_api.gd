class_name GameModeLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetGameModeDB", func(): return GameModeDB)
	lua.push_variant("GetGameModeSystem", func(): return GameModeSystem)


func expose_hooks(lua: LuaAPI) -> void:
	GameModeDB.game_mode_registered.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnGameModeRegistered", [mode])
	)
	GameModeDB.game_mode_unregistered.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnGameModeUnregistered", [mode])
	)
	GameModeSystem.game_mode_entered.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnGameModeEntered", [mode])
	)
	GameModeSystem.game_mode_exited.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnGameModeExited", [mode])
	)
	GameModeSystem.before_game_mode_entered.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnBeforeGameModeEntered", [mode])
	)
	GameModeSystem.before_game_mode_exited.connect(
		func(mode: GameModeResource) -> void:
			lua.call_function("OnBeforeGameModeExited", [mode])
	)
