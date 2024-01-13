class_name PlayerLuaAPI extends ExposeLuaAPI


func expose_variables(lua: LuaAPI) -> void:
	lua.push_variant("GetPlayerSystem", func(): return PlayerSystem)
	lua.push_variant("PlayerConfig", PlayerConfig)


func expose_hooks(lua: LuaAPI) -> void:
	PlayerSystem.player_spawned.connect(
		func(node: Node) -> void:
			lua.call_function("OnPlayerSpawned", [node])
	)
	PlayerSystem.player_despawned.connect(
		func() -> void:
			lua.call_function("OnPlayerDespawned", [])
	)
	PlayerSystem.before_player_spawned.connect(
		func() -> void:
			lua.call_function("OnBeforePlayerSpawned", [])
	)
	PlayerSystem.before_player_despawned.connect(
		func(node: Node) -> void:
			lua.call_function("OnBeforePlayerDespawned", [node])
	)
