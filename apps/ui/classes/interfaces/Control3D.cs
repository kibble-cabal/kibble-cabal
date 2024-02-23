using Godot;

public interface IPositioned2D
{
    Vector2 Position { get; set; }
}

public interface ISized2D
{
    Vector2 Size { get; }
}

public interface IControl3D : IPositioned2D, ISized2D
{
    Vector3 LocalPosition { get; set; }
    Vector2 ScreenOffset { get; set; }
    bool Center { get; set; }

    static void ProcessPosition<T>(T node, Node? parentNode, Camera3D? camera, bool enableCenter = true) where T : Node, IControl3D
    {
        if (!node.IsInsideTree() || parentNode is null || camera is null) return;
        var totalPositon = node.LocalPosition;
        if (parentNode is Node3D parent) totalPositon += parent.GlobalPosition;
        node.Position = camera.UnprojectPosition(totalPositon) + node.ScreenOffset;
        if (node.Center && enableCenter) node.Position -= node.Size / 2;
    }
}