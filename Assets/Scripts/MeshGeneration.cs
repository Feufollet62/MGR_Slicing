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

public static class Cutter
{
    public static bool cutting;
    public static Mesh OriginalMesh;

    public static void Cut(Transform originalObject, Vector3 contactPoint, Vector3 direction, Material material = null, bool fill = true, bool addRB = true)
    {
        if(cutting) return;

        cutting = true;
        OriginalMesh = originalObject.GetComponent<MeshFilter>().mesh;

        Plane plane = new Plane(
            originalObject.InverseTransformDirection(-direction), 
            originalObject.InverseTransformPoint(contactPoint));
        
        List<Vector3> addedVertices = new List<Vector3>();
        GeneratedMesh meshLeft = new GeneratedMesh();
        GeneratedMesh meshRight = new GeneratedMesh();
        int[] subMeshIndices;
        int triangleIndexA, triangleIndexB, triangleIndexC;

        for (int i = 0; i < OriginalMesh.subMeshCount; i++)
        {
            subMeshIndices = OriginalMesh.GetTriangles(i);

            for (int j = 0; j < subMeshIndices.Length; j+=3)
            {
                triangleIndexA = subMeshIndices[j];
                triangleIndexB = subMeshIndices[j+1];
                triangleIndexC = subMeshIndices[j+2];

                Triangle currentTriangle = GetTriangle(triangleIndexA, triangleIndexB, triangleIndexC, i);
                
                bool triangleALeft = plane.GetSide(OriginalMesh.vertices[triangleIndexA]);
                bool triangleBLeft = plane.GetSide(OriginalMesh.vertices[triangleIndexB]);
                bool triangleCLeft = plane.GetSide(OriginalMesh.vertices[triangleIndexC]);
                
                if(triangleALeft && triangleBLeft && triangleCLeft) meshLeft.AddTriangle(currentTriangle);
                else if (!triangleALeft && !triangleBLeft && !triangleCLeft) meshRight.AddTriangle(currentTriangle);
                else CutTriangle(plane, currentTriangle, triangleALeft, triangleBLeft, triangleCLeft, meshLeft,
                        meshRight, addedVertices);
            }
        }
    }
    
    private static Triangle GetTriangle(int triangleIndexA, int triangleIndexB, int triangleIndexC, int subMeshIndex)
    {
        Vector3[] vertices =
        {
            OriginalMesh.vertices[triangleIndexA],
            OriginalMesh.vertices[triangleIndexB],
            OriginalMesh.vertices[triangleIndexC]
        };
        
        Vector3[] normals =
        {
            OriginalMesh.normals[triangleIndexA],
            OriginalMesh.normals[triangleIndexB],
            OriginalMesh.normals[triangleIndexC]
        };
        
        Vector2[] uvs = 
        {
            OriginalMesh.uv[triangleIndexA],
            OriginalMesh.uv[triangleIndexB],
            OriginalMesh.uv[triangleIndexC]
        };

        return new Triangle(vertices, normals, uvs, subMeshIndex);
    }

    private static void CutTriangle
        (Plane plane, Triangle triangle, bool triangleALeft, bool triangleBLeft,bool triangleCLeft, 
        GeneratedMesh meshLeft, GeneratedMesh meshRight, List<Vector3> addedVertices)
    {
        
    }
}