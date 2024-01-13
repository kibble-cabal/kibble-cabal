# MusicSystem
extends Node


func _ready() -> void:
	LocationSystem.location_changed.connect(_on_location_changed)


func play_music(song: AudioStream) -> void:
	SoundManager.play_music_at_volume(song, get_music_volume(), get_music_fade_duration())


func stop_music() -> void:
	SoundManager.stop_music(get_music_fade_duration())


## NOTICE: Will be replaced by user setting
func get_music_volume() -> float:
	return 0.0


## NOTICE: Will be replaced by user setting
func get_music_fade_duration() -> float:
	return 1.0


func lua_fields() -> Array:
	return [
		"get_music_fade_duration",
		"get_music_volume",
		"play_music",
		"stop_music"
	]


func _on_location_changed() -> void:
	if not LocationSystem.current_location: return stop_music()
	
	var music := LocationSystem.current_location.get_music()
	if not music: return stop_music()
	
	var song := music.get_song()
	if song: play_music(song)
	else: stop_music()
