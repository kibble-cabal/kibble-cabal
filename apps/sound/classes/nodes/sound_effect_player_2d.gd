class_name SoundEffectPlayer2D extends AudioStreamPlayer2D

const SEMITONE = 1.059463

@export var semitones: int = 5

## If below 0.0, the stream's length is used as the loop length
@export var loop_length: float = -1.0

var timer := Timer.new()

func _init() -> void:
	timer.timeout.connect(randomize_pitch)


func _ready() -> void:
	_update_wait_time()
	add_child(timer)


func play_loop() -> void:
	timer.one_shot = false
	timer.timeout.connect(play)
	timer.start()
	play()


func stop_loop() -> void:
	timer.stop()
	timer.timeout.disconnect(play)


func play_once() -> void:
	timer.one_shot = true
	timer.start()
	play()


func mute() -> void:
	volume_db = -100


func unmute() -> void:
	volume_db = 0


func set_sound(sound: AudioStream) -> void:
	_update_wait_time()
	stream = sound


func randomize_pitch() -> void:
	pitch_scale = 1
	for i in range(range(1, semitones + 1).pick_random()):
		pitch_scale *= SEMITONE


func _update_wait_time() -> void:
	if stream and loop_length < 0:
		timer.wait_time = stream.get_length()
	else:
		timer.wait_time = loop_length
