using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RMusic : ExtensibleResource, IIdentifiable<StringName>
{
    public StringName _id = "";
    public Dictionary<StringName, AudioStream> _songs = [];
    public Script? _selectorScript;
    public StringName _selectorMethod = "";

    [Export]
    public StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public Dictionary<StringName, AudioStream> Songs
    {
        get => _songs;
        set => this.Set(ref _songs, value);
    }

    [Export]
    public Script? SelectorScript
    {
        get => _selectorScript;
        set => this.Set(ref _selectorScript, value);
    }

    /// <summary>
    /// Should have signature:
    /// <code>AudioStream? (RSave saveFile)</code>
    /// </summary>
    [Export]
    public StringName SelectorMethod
    {
        get => _selectorMethod;
        set => this.Set(ref _selectorMethod, value);
    }

    public AudioStream? GetSong()
    {
        if (SelectorScript is not null)
        {
            var selector = SelectorScript.New();
            if (selector!.HasMethod(SelectorMethod))
                return selector.Call(SelectorMethod).TryAs<AudioStream>();
        }
        return Songs.Values.FirstOrDefault();
    }
}