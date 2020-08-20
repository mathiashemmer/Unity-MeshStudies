using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator pathCreator;
    Path path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if(GUILayout.Button("Toggle Closed"))
        {
            pathCreator.ToggleClosed();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Reset Path"))
        {
            pathCreator.CreatePath();
            path = pathCreator.path;
        }

        if (GUILayout.Button("Hide nodes"))
            pathCreator.HideNodes = !pathCreator.HideNodes;
            
        path.MirrorControlls = GUILayout.Toggle(path.MirrorControlls, "Toggle Mirroring");
    }

    private void OnSceneGUI()
    {
        Draw();
        Input();
    }

    void Input()
    {
        var guiEvent = Event.current;

        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            AddSegment(guiEvent);
        else if(guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
            DeleteSegment(guiEvent);
        else if (guiEvent.type == EventType.KeyDown && Event.current.control && !path.MirrorControlls)
            path.MirrorControlls = true;
        else if (guiEvent.type == EventType.KeyUp && !Event.current.control && path.MirrorControlls)
            path.MirrorControlls = false;
    }

    void Draw()
    { 
        for(int i = 0; i < path.SegmentsCount; i++)
        {
            var points = path.GetPointsInSegment(i);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], pathCreator.SegmentColor, null, 2);
            Handles.color = Color.black;
            if (!pathCreator.HideNodes)
            {
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            
        }

        if (!pathCreator.HideNodes)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var isAnchor = i % 3 == 0;

                Handles.color = isAnchor ? pathCreator.AnchorColor : pathCreator.ControllerColor;
                var size = isAnchor ? pathCreator.AnchorSize : pathCreator.ControllerSize;

                Vector3 position = Handles.FreeMoveHandle(path[i], Quaternion.identity, size, pathCreator.SnapInterval, Handles.SphereHandleCap);
                if (path[i] != position)
                {
                    Undo.RecordObject(pathCreator, "Move point");
                    path.MovePoint(i, position);
                }
            }
        }
    }

    void OnEnable()
    {
        pathCreator = (PathCreator)target;

        if (pathCreator.path == null)
            pathCreator.CreatePath();

        path = pathCreator.path;
    }

    private void AddSegment(Event guiEvent)
    {
        var mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Vector3 mousePosition = mouseRay.origin + mouseRay.direction * 2;

        Undo.RecordObject(pathCreator, "Add segment");
        path.AddSegment(mousePosition);
    }

    private void DeleteSegment(Event guiEvent)
    {
        var minDistanceToAnchor = pathCreator.AnchorSize;
        var indexToDelete = -1;

        var mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

        for (int i = 0; i < path.Count && indexToDelete == -1; i += 3)
        {
            for(int sample = 0; sample < 100 && indexToDelete == -1; sample++)
            {
                var samplePoint = mouseRay.origin + (mouseRay.direction * sample * pathCreator.AnchorSize);
                if (Vector3.Distance(samplePoint, path[i]) < pathCreator.AnchorSize + 0.05f)
                    indexToDelete = i;
            }
        }

        if (indexToDelete != -1)
        {
            Undo.RecordObject(pathCreator, "Delete segment");
            path.DeleteSegment(indexToDelete);
        }
    }
}
