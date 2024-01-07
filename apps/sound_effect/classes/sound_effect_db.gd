# SoundEffectDB
extends Node

signal sound_effect_registered(sound_effect: AudioStream)
signal sound_effect_unregistered(sound_effect: AudioStream)


var registered_sound_effects: Array[AudioStream] = []


func register(sound_effect: AudioStream) -> void:
	registered_sound_effects.append(sound_effect)
	sound_effect_registered.emit(sound_effect)


func unregister(sound_effect: AudioStream) -> void:
	registered_sound_effects.erase(sound_effect)
	sound_effect_unregistered.emit(sound_effect)


func find_by_path(sound_effect_path: String) -> AudioStream:
	for sound_effect in registered_sound_effects:
		if sound_effect.resource_path == sound_effect_path: return sound_effect
	return null
