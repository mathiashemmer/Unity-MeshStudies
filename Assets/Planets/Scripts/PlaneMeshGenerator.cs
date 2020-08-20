using UnityEngine;

public class PlaneMeshGenerator : MonoBehaviour
{
    public float size = 1;
    public int resolution = 1;

    public void OnValidate() {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();

        mesh.vertices = GenerateVertex();
        mesh.uv = GenerateUVs(mesh.vertices);
        mesh.triangles = GenerateTriangles(mesh.vertices);
        mesh.RecalculateNormals();
    }

    private Vector3[] GenerateVertex()
    {

        var vertexRowLenght = resolution + 1;
        var vertices = new Vector3[vertexRowLenght * vertexRowLenght];

        var index = 0;
        
        for (int x = 0; x < vertexRowLenght; x += 1)
        {
            for (int z = 0; z < vertexRowLenght; z += 1)
            {
                var offset = (vertexRowLenght-1) / 2f;
                vertices[index] = new Vector3((x- offset) * size/resolution, 0, (z- offset) * size/resolution);
                index++;
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles(Vector3[] vertices)
    {
        int[] triangles = new int[6 * vertices.Length];
 
        var currentIndex = 0;
        var vertexCount = 0;
        for (int x =0; x < resolution; x++)
        {
            for (int z=0; z < resolution; z++)
            {
                triangles[currentIndex + 0] = vertexCount;
                triangles[currentIndex + 1] = vertexCount + 1;
                triangles[currentIndex + 2] = vertexCount + resolution + 1;
                triangles[currentIndex + 3] = vertexCount + 1;
                triangles[currentIndex + 4] = vertexCount + 1 + resolution + 1;
                triangles[currentIndex + 5] = vertexCount + resolution + 1;

                currentIndex += 6;
                
                vertexCount++;
            }
            vertexCount++;
        }

        return triangles;
    }

    private Vector2[] GenerateUVs(Vector3[] vertices)
    {
        var uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        return uvs;
    }

}
