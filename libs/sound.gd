class_name Sound

const SEMITONE = 1.059463


## The higher [code]semitones[/code] is, the more variation there will be in the random pitch
static func random_pitch(semitones: int = 5) -> float:
	var pitch_scale: float = 1.0
	for i in range(range(1, semitones + 1).pick_random()):
		pitch_scale *= SEMITONE
	return pitch_scale
