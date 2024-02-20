using Godot;

public static class Camera3DExtensions
{

    public static Vector3 ProjectToFloor(this Camera3D camera, Vector2 position)
    {
        var origin = camera.ProjectRayOrigin(position);
        var direction = camera.ProjectRayNormal(position);
        var distance = -origin.Y / (direction.Y != 0 ? direction.Y : 1);
        return origin + direction * distance;
    }
}