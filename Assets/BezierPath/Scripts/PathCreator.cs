using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector]
    public Path path;

    public Color AnchorColor = Color.red;
    public Color ControllerColor = Color.white;
    public Color SegmentColor = Color.green;
    public Color SegmentHighlightColor = Color.yellow;

    public Vector3 SnapInterval = Vector3.zero;
    public float AnchorSize = 0.1f;
    public float ControllerSize = 0.05f;

    [HideInInspector]
    public bool HideNodes;

    public delegate void PathUpdate();

    public void CreatePath()
    {
        path = new Path(transform.position);
    }

    public void ToggleClosed()
    {
        path.Closed = !path.Closed;
    }

}
