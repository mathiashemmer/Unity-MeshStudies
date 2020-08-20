using UnityEngine;
public class CubeMeshGenerator : MonoBehaviour
{
    public float size = 1;
    public int resolution = 1;

    MeshFilter[] meshFilters;
    Noise noise;

    public void GenerateMesh()
    {
        noise = new Noise();

        if(meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[6];

        var directions = new Vector3[]{ Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        for(int face = 0; face < 6; face++)
        {
            if(meshFilters[face] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[face] = meshObj.AddComponent<MeshFilter>();
                meshFilters[face].sharedMesh = new Mesh();
            }
            
            meshFilters[face].sharedMesh.Clear();
            meshFilters[face].sharedMesh.vertices = GenerateVertex(directions[face]);
            meshFilters[face].sharedMesh.triangles = GenerateTriangles(meshFilters[face].sharedMesh.vertices);
            meshFilters[face].sharedMesh.RecalculateNormals();
        }

    }

    private Vector3[] GenerateVertex(Vector3 direction)
    {
        var axisA = new Vector3(direction.y, direction.z, direction.x);
        var axisB = Vector3.Cross(direction, axisA);

        var vertexRowLenght = resolution + 1;
        var vertices = new Vector3[vertexRowLenght * vertexRowLenght];

        var index = 0;

        for (int i = 0; i < vertexRowLenght; i += 1)
        {
            for (int j = 0; j < vertexRowLenght; j += 1)
            {
                var percent = new Vector2(i , j) / resolution;
                var pointInCube = (direction + (percent.x - 0.5f)*2 * axisA + (percent.y - 0.5f)*2 * axisB);
                var pointInCircle = pointInCube.normalized;
                //pointInCircle *= size * (1 + (1 + noise.Evaluate(new Vector3(pointInCube.x, pointInCube.y, pointInCube.z)) * .5f));
                vertices[index] = pointInCircle;
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
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                triangles[currentIndex + 0] = vertexCount;
                triangles[currentIndex + 1] = vertexCount + resolution + 1;
                triangles[currentIndex + 2] = vertexCount + 1;
                triangles[currentIndex + 3] = vertexCount + 1;
                triangles[currentIndex + 4] = vertexCount + resolution + 1;
                triangles[currentIndex + 5] = vertexCount + resolution + 2;

                currentIndex += 6;

                vertexCount++;
            }
            vertexCount++;
        }

        return triangles;
    }
}
