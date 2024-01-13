# MusicDB
extends Node

signal music_registered(music: MusicResource)
signal music_unregistered(music: MusicResource)


var registered_music: Array[MusicResource] = []


func register(music: MusicResource) -> void:
	registered_music.append(music)
	music_registered.emit(music)


func unregister(music: MusicResource) -> void:
	registered_music.erase(music)
	music_unregistered.emit(music)


func find(music_id: String) -> MusicResource:
	for music in registered_music:
		if music.id == music_id: return music
	return null


func lua_fields() -> Array:
	return ["register", "unregister", "find", "registered_music"]
