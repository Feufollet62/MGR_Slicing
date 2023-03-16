using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration
{
    
}

public class GeneratedMesh
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> Uvs = new List<Vector2>();
    public List<List<int>> SubMeshIndices = new List<List<int>>();

    public void AddTriangle(Triangle triangle)
    {
        Vertices.AddRange(triangle.Vertices);
        Normals.AddRange(triangle.Normals);
        Uvs.AddRange(triangle.Uvs);

        if (SubMeshIndices.Count < triangle.SubmeshIndex + 1)
        {
            for (int i = SubMeshIndices.Count; i < triangle.SubmeshIndex + 1; i++)
            {
                SubMeshIndices.Add(new List<int>());
            }
        }

        for (int i = 0; i < 3; i++)
        {
            SubMeshIndices[triangle.SubmeshIndex].Add(Vertices.Count + i);
        }
    }
}

public class Triangle
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> Uvs = new List<Vector2>();
    public int SubmeshIndex;

    public Triangle(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int submeshIndex)
    {
        Clear();
        
        Vertices.AddRange(vertices);
        Normals.AddRange(normals);
        Uvs.AddRange(uvs);

        SubmeshIndex = submeshIndex;
    }

    private void Clear()
    {
        Vertices.Clear();
        Normals.Clear();
        Uvs.Clear();

        SubmeshIndex = 0;
    }
}