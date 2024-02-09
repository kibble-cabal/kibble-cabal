using System;
using System.Linq;
using Godot;

[Tool]
[GlobalClass]
public partial class PolylineMesh : PolylineMeshBase
{
    [Export]
    public Vector2[] points
    {
        get => Points;
        set
        {
            Points = value;
            InternalMesh.Generate(this);
        }
    }
}
