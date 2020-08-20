using UnityEngine;

[CreateAssetMenu(menuName = "NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    public float Amplitude;
    public float Frequency;

    public float Evaluate(float index)
    {
        var point = index * Frequency;
        return (Mathf.PerlinNoise(point, 1)*2-1) * Amplitude;
    }

    

}

public class DayTradeChart : MonoBehaviour
{
    public float offsetY = 0f;
    [Range(1,10)]
    public int DataAmmount = 1;
    public int SampleRate = 5;
    [Range(1f, 100f)]
    public float Resolution = 1;
    public float Scale = 1;

    public NoiseSettings[] noiseSettings;

    private void OnValidate()
    {
        GeneratePoints();
    }

    

    public void GeneratePoints()
    {
        var points = new Vector3[DataAmmount * SampleRate];
        for(int x = 0; x < DataAmmount; x++)
        {
            for(int y = 0; y < SampleRate; y++)
            {
                var posY = 0f;
                var index = y * DataAmmount + x;
                foreach (var noise in noiseSettings)
                    posY += noise.Evaluate(index / (float)SampleRate);
                points[index] += new Vector3(index / (float)SampleRate, offsetY + posY, 0) * Scale;

            }
        }

        gameObject.GetComponent<LineRenderer>().positionCount = (DataAmmount * SampleRate);
        gameObject.GetComponent<LineRenderer>().SetPositions(points);
    }
}
