using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlacer : MonoBehaviour
{
    public float spacing = .1f;
    public float resolution = 1;
    void Start()
    {
        var points = FindObjectOfType<PathCreator>().path.GetDiscretePointsInterval(spacing, resolution);
        foreach(var p in points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = Vector3.one * spacing * 0.5f;
        }
    }

}
