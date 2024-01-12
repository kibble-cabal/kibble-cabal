extends "res://addons/sound_manager/abstract_audio_player_pool.gd"


func play(resource: AudioStream, position: float = 0.0, volume: float = 0.0, crossfade_duration: float = 0.0, override_bus: String = "") -> AudioStreamPlayer:
	stop_all(crossfade_duration * 2)

	var player := play(resource, position, volume, crossfade_duration, override_bus)

	# Remember this track name
	track_history.insert(0, resource.resource_path)
	if track_history.size() > 50:
		track_history.remove_at(50)

	player.call_deferred("play", position)
	return player

