extends Node


@export var default_busses := []
@export var default_pool_size := 8

var tweens: Dictionary = {}
var track_history: PackedStringArray = []


var available_players: Array[AudioStreamPlayer] = []
var busy_players: Array[AudioStreamPlayer] = []
var bus: String = "Master"


func _init(possible_busses: PackedStringArray = default_busses, pool_size: int = default_pool_size) -> void:
	bus = get_possible_bus(possible_busses)

	for i in pool_size:
		increase_pool()

func get_possible_bus(possible_busses: PackedStringArray) -> String: 
	for possible_bus in possible_busses:
		var cases: PackedStringArray = [
			possible_bus,
			possible_bus.to_lower(),
			possible_bus.to_camel_case(),
			possible_bus.to_pascal_case(),
			possible_bus.to_snake_case()
		]
		for case in cases:
			if AudioServer.get_bus_index(case) > -1:
				return case
	return "Master"

func prepare(resource: AudioStream, override_bus: String = "") -> AudioStreamPlayer:
	var player: AudioStreamPlayer

	if resource is AudioStreamRandomizer:
		player = get_player_with_resource(resource)

	if player == null:
		player = get_available_player()

	player.stream = resource
	player.bus = override_bus if override_bus != "" else bus
	player.volume_db = linear_to_db(1.0)
	player.pitch_scale = 1
	return player


func get_available_player() -> AudioStreamPlayer:
	if available_players.size() == 0:
		increase_pool()
	var player = available_players.pop_front()
	busy_players.append(player)
	return player


func get_player_with_resource(resource: AudioStream) -> AudioStreamPlayer:
	for player in busy_players + available_players:
		if player.stream == resource:
			return player
	return null


func mark_player_as_available(player: AudioStreamPlayer) -> void:
	if busy_players.has(player):
		busy_players.erase(player)

	if not available_players.has(player):
		available_players.append(player)


func increase_pool() -> void:
	var player := AudioStreamPlayer.new()
	add_child(player)
	available_players.append(player)
	player.bus = bus
	player.finished.connect(_on_player_finished.bind(player))


func stop(resource: AudioStream, fade_out_duration: float = 0.0) -> void:
	var player := _get_player_with_stream(resource)
	if player:
		if fade_out_duration <= 0.0:
			fade_out_duration = 0.01
		fade_volume(player, player.volume_db, -80, fade_out_duration)


func stop_all(fade_out_duration: float = 0.0) -> void:
	for player in busy_players:
		if fade_out_duration <= 0.0:
			fade_out_duration = 0.01
		fade_volume(player, player.volume_db, -80, fade_out_duration)


func play(resource: AudioStream, position: float = 0.0, volume: float = 0.0, crossfade_duration: float = 0.0, override_bus: String = "") -> AudioStreamPlayer:
	var player = _get_player_with_stream(resource)

	# If the player already exists then just make sure the volume is right (it might have just been fading in or out)
	if player != null:
		fade_volume(player, player.volume_db, volume, crossfade_duration)
		return player
		
	# Otherwise we need to prep another player and handle its introduction
	player = prepare(resource, override_bus)
	fade_volume(player, -80.0, volume, crossfade_duration)
	
	player.call_deferred("play", position)
	return player


func is_playing(resource: AudioStream) -> bool:
	if resource != null:
		return _get_player_with_stream(resource) != null
	else:
		return busy_players.size() > 0


func pause(resource: AudioStream = null) -> void:
	if resource != null:
		var player = _get_player_with_stream(resource)
		if is_instance_valid(player):
			player.stream_paused = true
	else:
		for player in busy_players:
			player.stream_paused = true


func resume(resource: AudioStream = null) -> void:
	if resource != null:
		var player = _get_player_with_stream(resource)
		if is_instance_valid(player):
			player.stream_paused = false
	else:
		for player in busy_players:
			player.stream_paused = false


func is_track_playing(resource_path: String) -> bool:
	for player in busy_players:
		if player.stream.resource_path == resource_path:
			return true
	return false


func get_currently_playing() -> Array[AudioStream]:
	var tracks: Array[AudioStream] = []
	for player in busy_players:
		tracks.append(player.stream)
	return tracks


func get_currently_playing_tracks() -> PackedStringArray:
	var tracks: PackedStringArray = []
	for player in busy_players:
		tracks.append(player.stream.resource_path)
	return tracks


func fade_volume(player: AudioStreamPlayer, from_volume: float, to_volume: float, duration: float) -> AudioStreamPlayer:
	# Remove any tweens that might already be on this player
	_remove_tween(player)

	# Start a new tween
	var tween: Tween = get_tree().create_tween().bind_node(self)

	player.volume_db = from_volume
	if from_volume > to_volume:
		# Fade out
		tween.tween_property(player, "volume_db", to_volume, duration).set_trans(Tween.TRANS_CIRC).set_ease(Tween.EASE_IN)
	else:
		# Fade in
		tween.tween_property(player, "volume_db", to_volume, duration).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)

	tweens[player] = tween
	tween.finished.connect(_on_fade_completed.bind(player, tween, from_volume, to_volume, duration))

	return player


### Helpers


func _get_player_with_stream(resource: AudioStream) -> AudioStreamPlayer:
	for player in busy_players:
		if player.stream == resource:
			return player
	return null


func _remove_tween(player: AudioStreamPlayer) -> void:
	if tweens.has(player):
		var fade: Tween = tweens.get(player)
		fade.kill()
		tweens.erase(player)


### Signals


func _on_fade_completed(player: AudioStreamPlayer, tween: Tween, from_volume: float, to_volume: float, duration: float):
	_remove_tween(player)

	# If we just faded out then our player is now available
	if to_volume <= -79.0:
		player.stop()
		mark_player_as_available(player)


func _on_player_finished(player: AudioStreamPlayer) -> void:
	mark_player_as_available(player)


func lua_fields() -> Array:
	return [
		"available_players",
		"bus",
		"busy_players",
		"default_busses",
		"default_pool_size",
		"fade_volume",
		"get_available_player",
		"get_currently_playing",
		"get_currently_playing_tracks",
		"get_player_with_resource",
		"get_possible_bus",
		"increase_pool",
		"is_playing",
		"is_track_playing",
		"mark_player_as_available",
		"pause",
		"play",
		"prepare",
		"resume",
		"stop",
		"stop_all",
		"track_history"
	]
