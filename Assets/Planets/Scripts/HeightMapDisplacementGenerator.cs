using UnityEngine;

public class HeightMapDisplacementGenerator : MonoBehaviour
{
    [Header("Perlin Noise Settings")]
    public int Octaves = 1;
    public float Lacunarity = 1;
    public float Persistence = 1;

    public int Seed = 1337;
    public float OffsetX = 1;
    public float OffsetY = 1;
    public float Scale = 0.2f;

    public AnimationCurve anim;
    public float heightMultiplier = 10f;
    public float[,] GeneratePerlinValue(int size)
    {

        var heightMap = new float[size, size];
        float maxHight = float.MinValue;
        float minHight = float.MaxValue;

        System.Random prng = new System.Random(Seed);

        Vector2[] octavesOffset = new Vector2[Octaves];
        for(int i = 0; i < Octaves; i++)
            octavesOffset[i] = new Vector2(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
        


        for (int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                float value = 0f;
                float frequency = 1;
                float amplitude = 1;

                for (int indexX = 0; indexX < Octaves; indexX++)
                {
                    var sampleX = (x + OffsetX) / Scale * frequency + octavesOffset[indexX].x;
                    var sampleY = (y + OffsetY) / Scale * frequency + octavesOffset[indexX].y;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    value += perlinValue * amplitude;

                    amplitude *= Persistence;
                    frequency *= Lacunarity;
                }

                if (value > maxHight)
                    maxHight = value;
                if (value < minHight)
                    minHight = value;

                heightMap[x,y] = value;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                heightMap[x, y] = Mathf.InverseLerp(minHight, maxHight, heightMap[x, y]);
            }
        }

        return heightMap;

    }
}
