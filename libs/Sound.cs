using Godot;

public static class Sound
{
    public const double Semitone = 1.059463;

    /// <summary>
    /// Returns a semi-random pitch.
    /// </summary>
    /// <param name="semitones">The higher semitones is, the more variation there will be in the random pitch.</param>
    public static double RandomPitch(int semitones = 5)
    {
        var pitchScale = 1.0;
        var max = Mathf.RoundToInt(GD.RandRange(1, semitones + 1));
        for (var i = 0; i < max; i++) pitchScale *= Semitone;
        return pitchScale;
    }
}