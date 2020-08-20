using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMeshCreator : MonoBehaviour
{
    public int Resolution = 6;
    public float Radius = 1f;

    public float PathResolution = 1f;
    public float PathSpacing = 1f;

    public Vector3[] Vertices;
    public int[] Triangles;

    public void CreateMesh()
    {
        var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        var pathCreator = gameObject.GetComponent<PathCreator>();
        
        if (mesh == null)
            mesh = new Mesh();

        mesh.Clear();
        GenerateVertices(pathCreator.path.GetDiscretePointsInterval(PathSpacing, PathResolution));
        GenerateTriangles();

        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    public void GenerateVertices(Vector3[] points)
    {
        Vertices = new Vector3[points.Length * Resolution];
        var baseDirection = Vector3.forward;
        for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            if(pointIndex < points.Length - 1)
                baseDirection = points[pointIndex + 1] - points[pointIndex];

            for (int i = 0; i < Resolution; i++)
            {
                var rotation = Quaternion.AngleAxis((360 / Resolution) * i, baseDirection.normalized);
                var pointInUnitCircle = (rotation * Vector3.Cross(Vector3.up, baseDirection.normalized).normalized * Radius) + points[pointIndex];
                Vertices[Resolution * pointIndex + i] = pointInUnitCircle;
            }
        }
    }

    public void GenerateTriangles()
    {
        Triangles = new int[6*(Vertices.Length - Resolution)];

        var vertexCount = 0;
        for (int i = 0; i < Triangles.Length; i+=6)
        {
            if((vertexCount+1) % Resolution == 0)
            {
                Triangles[i + 0] = vertexCount;
                Triangles[i + 1] = vertexCount - Resolution + 1;
                Triangles[i + 2] = vertexCount + Resolution;
                Triangles[i + 3] = vertexCount + Resolution;
                Triangles[i + 4] = vertexCount - Resolution + 1;
                Triangles[i + 5] = vertexCount + 1;
            }
            else
            {
                Triangles[i + 0] = vertexCount;
                Triangles[i + 1] = vertexCount + 1;
                Triangles[i + 2] = vertexCount + Resolution;
                Triangles[i + 3] = vertexCount + Resolution;
                Triangles[i + 4] = vertexCount + 1;
                Triangles[i + 5] = vertexCount + Resolution + 1;
            }
            vertexCount++;
        }
    }


}
