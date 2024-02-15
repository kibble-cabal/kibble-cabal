using Godot;


public interface IBuildingComponent<T> : IGodotSerializable<T>
{
    /* Properties */
    MaterialMap Materials { get; set; }

    /* Unimplemented methods */

    int GetIndex(Building building);
    bool IsValid();
    Vector2[] Tessellate();
    bool IsTouching(T other, float threshold);
    Rect2 GetBoundingBox();
    Vector2 ClosestPoint(Vector2 position);
    Vector2 ClosestPointOnSurface(Vector2 position);
    Mesh[] GenerateMeshes(Building building);

}

public static class BuildingComponentExtensions
{
    public static StringName GetMaterialID<T>(this T component, StringName materialName) where T : IBuildingComponent<T> => component.Materials.ContainsKey(materialName) ? component.Materials[materialName] : new();
    public static Vector2 GetCentroid<T>(this T component) where T : IBuildingComponent<T> => component.GetBoundingBox().GetCenter();
    public static Vector2 Snap<T>(this T component, Vector2 position, float threshold = -1) where T : IBuildingComponent<T> => position.Snap(component.ClosestPoint(position), threshold);
    public static Vector2 SnapToSurface<T>(this T component, Vector2 position, float threshold = -1) where T : IBuildingComponent<T> => position.Snap(component.ClosestPointOnSurface(position), threshold);
}