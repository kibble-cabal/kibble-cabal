using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BuildingMesh : ArrayMesh
{
    private ProceduralMesh InternalMesh;
    private bool IsGenerating = false;

    private RBuilding Building;

    private readonly List<VolumePolyline> WallMeshes = [];
    private readonly List<IMeshComponent> FloorMeshes = [];
    private readonly List<IMeshComponent> RoofMeshes = [];

    public BuildingMesh(RBuilding building)
    {
        Building = building;
        building.WallAdded += OnWallAdded;
        building.FloorAdded += OnFloorAdded;
        building.RoofAdded += OnRoofAdded;
        building.WallRemoved += (index, _) => OnWallRemoved(index);
        building.FloorRemoved += (index, _) => OnFloorRemoved(index);
        building.RoofRemoved += (index, _) => OnRoofRemoved(index);
        InternalMesh = new ProceduralMesh(GetComponents, this);
        building.GetValid<Wall>().ForEach(wall => AddWall(wall, null));
        for (int i = 0; i < building.Floors.Count; i++)
            OnFloorAdded(i);
        for (int i = 0; i < building.Roofs.Count; i++)
            OnRoofAdded(i);
    }

    private void UpdateWalls() => Building.Walls.ForEach((_, index) => UpdateWallMesh(index));

    private void UpdateWallMesh(int index, Action<Wall, VolumePolyline> updateFn)
    {
        if (Building.Get<Wall>(index) is Wall wall && WallMeshes.Count > index)
        {
            updateFn(wall, WallMeshes[index]);
            Generate();
        }
    }

    private void UpdateWallMesh(int index) => UpdateWallMesh(index, (wall, mesh) =>
    {
        mesh.Surface = index * 5;
        mesh.Points = wall.Tessellate();
        mesh.Thickness = wall.Thickness;
        mesh.ExtrudeAmount = wall.Height;
        mesh.JoinStart = wall.GetJoin(Building, wall.Start);
        mesh.JoinEnd = wall.GetJoin(Building, wall.End);
    });

    private void OnWallAdded(int index)
    {
        if (Building.Get<Wall>(index) is Wall wall)
            AddWall(wall, index);
    }

    private void AddWall(Wall wall, int? i = null)
    {
        var index = i ?? Building.GetIndex<Wall>(wall);
        var mesh = wall.GetMeshComponent(Building);
        if (WallMeshes.Count <= index) WallMeshes.Add(mesh);
        else WallMeshes[index] = mesh;
        wall.StartChanged += (_, _) => UpdateWalls();
        wall.EndChanged += (_, _) => UpdateWalls();
        wall.StartHandleChanged += (_, _) => UpdateWalls();
        wall.EndHandleChanged += (_, _) => UpdateWalls();
        wall.ThicknessChanged += (_, value) => UpdateWallMesh(index, (wall, mesh) => mesh.Thickness = value);
        wall.HeightChanged += (_, value) => UpdateWallMesh(index, (wall, mesh) => mesh.ExtrudeAmount = value);
        Building.WallAdded += newIndex => UpdateWalls();
        Building.WallRemoved += (removedIndex, _) => UpdateWalls();
        mesh.Surface = index * 5;
        Generate();
    }

    private void OnFloorAdded(int index) { }
    private void OnRoofAdded(int index) { }

    private void OnWallRemoved(int index)
    {
        WallMeshes.RemoveAt(index);
        Generate();
    }

    private void OnFloorRemoved(int index) { }
    private void OnRoofRemoved(int index) { }

    private IMeshComponent[] GetComponents() => [
        ..WallMeshes.SelectMany(mesh => mesh.GetComponents()),
    ];

    private Material[] GetMaterials() => [
        ..Building.GetValid<Wall>().SelectMany(wall => Wall.GetMaterials()),
    ];

    public void Generate()
    {
        InternalMesh.OverrideMaterials = GetMaterials();
        InternalMesh.Generate(this);
    }
}