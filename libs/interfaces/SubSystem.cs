using Godot;

public interface ISaveFileSubSystem
{
    void _OnBeforeSaveFileEntered() { }
    void _OnAfterSaveFileEntered() { }
    void _OnBeforeSaveFileExited() { }
    void _OnAfterSaveFileExited() { }
}