using System.Linq;
using Godot;

public static class MeshExtensions
{
    public static void DebugDrawMesh(this Mesh mesh, Transform3D transform, float size, Color color)
    {
        var points = mesh.GetFaces();
        var t = transform.AffineInverse();
        for (int i = 0; i < points.Length; i += 3)
        {
            Vector3[] face = [
                points[i] * t,
                points[i + 1] * t,
                points[i + 2] * t,
                points[i] * t
            ];
            DebugDraw3D.DrawPointPath(face, DebugDraw3D.PointType.TypeSphere, size, color, color * 0.75f);
        }
    }

    /// <summary>
    /// Returns the three vertices that make up the face closest to localPos on the provided mesh.
    /// </summary>
    public static Vector3[] GetClosestFace(this Mesh mesh, Vector3 localPos)
    {
        var dist = float.PositiveInfinity;
        var closestFace = -1;
        var faces = mesh.GetFaces();
        for (int i = 0; i < faces.Length; i += 3)
        {
            var dist1 = faces[i].DistanceTo(localPos).Abs();
            var dist2 = faces[i + 1].DistanceTo(localPos).Abs();
            var dist3 = faces[i + 2].DistanceTo(localPos).Abs();
            var currentDist = dist1 + dist2 + dist3;
            if (currentDist < dist)
            {
                dist = currentDist;
                closestFace = i;
            }
        }
        if (closestFace >= 0)
            return [faces[closestFace], faces[closestFace + 1], faces[closestFace + 2]];
        return [];
    }

    public static Vector3 GetClosestPoint(this Mesh mesh, Vector3 localPos)
    {
        var face = mesh.GetClosestFace(localPos);
        if (face.Length >= 3)
        {
            var weights = Geometry3D
                .GetTriangleBarycentricCoords(localPos, face[0], face[1], face[2])
                .ClampBarycentricCoords(face[0], face[1], face[2]);
            return weights.X * face[0] + weights.Y * face[1] + weights.Z * face[2];
        }
        return localPos;
    }

    public static Vector3 GetClosestVertex(this Mesh mesh, Vector3 localPos) => mesh.GetFaces().Closest(localPos);
}