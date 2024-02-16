public enum GodotSerializationError
{
    IncorrectData
}

/// <summary>
/// This interface allows a native C# type to be converted to and from Godot-serializable data.
/// </summary>
/// <typeparam name="T">The struct for which this interace is being implemented.</typeparam>
public interface IGodotSerializable<T>
{
    Godot.Collections.Array Serialize();
    static abstract Result<T, GodotSerializationError> Deserialize(Godot.Collections.Array data);
}