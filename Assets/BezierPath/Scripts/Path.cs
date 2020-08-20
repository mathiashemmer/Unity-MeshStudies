using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField, HideInInspector]
    List<Vector3> Points;
    [SerializeField, HideInInspector]
    bool _Closed;
    public bool Closed {
        get 
        {
            return _Closed;
        }
        set 
        {
            _Closed = value;
            if (_Closed)
            {
                Points.Add(2 * Points[Points.Count - 1] - Points[Points.Count - 2]);
                Points.Add(2 * Points[0] - Points[1]);
            }
            else
            {
                Points.RemoveRange(Points.Count - 2, 2);
            }
        } 
    }
    [SerializeField, HideInInspector]
    bool _MirrorControlls;
    public bool MirrorControlls
    {
        get
        {
            return _MirrorControlls;
        }
        set
        {
            _MirrorControlls = value;
        }
    }
    public Vector3 this[int i]
    {
        get
        {
            return Points[i];
        }
    }
    public int Count
    {
        get
        {
            return Points.Count;
        }
    }
    public int SegmentsCount
    {
        get
        {
            return Points.Count/3;
        }
    }
    public Path(Vector3 center)
    {
        Points = new List<Vector3>()
        {
            center + Vector3.left,
            center + (Vector3.left + Vector3.up) *.5f,
            center + (Vector3.right + Vector3.down) *.5f,
            center + Vector3.right
        };
    }

    private int LoopIndex(int index)
    {
        return (index + Points.Count) % Points.Count;
    }

    public void AddSegment(Vector3 anchorPosition)
    {
        if (Closed) return;
        Points.Add(2*Points[Points.Count - 1] - Points[Points.Count - 2]);
        Points.Add((Points[Points.Count - 1] + anchorPosition)* 0.5f);
        Points.Add(anchorPosition);
    }

    public void DeleteSegment(int index)
    {
        if (SegmentsCount > 2 || (!Closed && SegmentsCount > 1))
        {
            if (index == 0)
            {
                if (Closed)
                    Points[Points.Count - 1] = Points[2];
                Points.RemoveRange(0, 3);
            }
            else if (index == Points.Count - 1 || !Closed)
                Points.RemoveRange(index - 2, 3);
            else
                Points.RemoveRange(index - 1, 3);
        }
    }

    public Vector3[] GetPointsInSegment(int index)
    {
        return new Vector3[]
        {
            Points[index * 3],
            Points[index * 3 + 1],
            Points[index * 3 + 2],
            Points[LoopIndex(index * 3 + 3)]
        };
    }

    public void MovePoint(int index, Vector3 position)
    {
        Vector3 deltaMove = position - Points[index];
        Points[index] = position;

        if (index % 3 == 0)
        {
            if (index + 1 < Points.Count || Closed)
                Points[LoopIndex(index + 1)] += deltaMove;
            if (index - 1 >= 0 || Closed)
                Points[LoopIndex(index - 1)] += deltaMove;
        }
        else if(MirrorControlls)
        {
            var nextPointIsAnchor = (index + 1) % 3 == 0;
            var correspondigControllToMirror = nextPointIsAnchor ? index + 2 : index - 2;
            var correspondingAnchorIndex = nextPointIsAnchor ? index + 1 : index - 1;
            if (correspondigControllToMirror >= 0 && correspondigControllToMirror < Points.Count || Closed)
            {
                var vectorToAnchor = Points[LoopIndex(correspondingAnchorIndex)] - Points[index];
                Points[LoopIndex(correspondigControllToMirror)] = Points[LoopIndex(correspondingAnchorIndex)] + vectorToAnchor;
            }
        }
    }

    public Vector3[] GetDiscretePointsInterval(float spacing, float resolution = 1)
    {
        List<Vector3> points = new List<Vector3>();
        var previousPoint = Points[0];
        float distanceSinceLastPoint = 0f;
        points.Add(Points[0]);

        for(int i = 0; i < SegmentsCount; i++)
        {
            var p = GetPointsInSegment(i);
            float controlNetLenght = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
            float estimatedCurveLenght = Vector3.Distance(p[0], p[3]) + controlNetLenght * .5f;
            int divisions = Mathf.CeilToInt(estimatedCurveLenght * resolution * 10);
            float t = 0;
            while(t <= 1f)
            {
                t += 1f/ divisions;
                var currentPoint = PathBezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                distanceSinceLastPoint += Vector3.Distance(previousPoint, currentPoint);

                while(distanceSinceLastPoint >= spacing)
                {
                    var overshot = distanceSinceLastPoint - spacing;
                    var newPoint = currentPoint + (previousPoint - currentPoint).normalized * overshot;
                    points.Add(newPoint);
                    distanceSinceLastPoint = overshot;
                    previousPoint = newPoint;
                }

                previousPoint = currentPoint;
            }
        }

        return points.ToArray();
    }
}

public static class PathBezier
{
    public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p1, p2, t);
    }

    public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p1 = EvaluateQuadratic(a, b, c, t);
        Vector3 p2 = EvaluateQuadratic(b, c, d, t);
        return Vector3.Lerp(p1, p2, t);
    }
}