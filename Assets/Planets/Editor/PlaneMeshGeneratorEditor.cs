using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaneMeshGenerator))]
public class PlaneMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var currentTarget = (PlaneMeshGenerator)target;
        
        currentTarget.size = EditorGUILayout.Slider("Size", currentTarget.size, 0.1f, 100f);
        currentTarget.resolution = EditorGUILayout.IntSlider("Resolution", currentTarget.resolution, 1, 255);

        if (GUILayout.Button("Generate") || GUI.changed)
            currentTarget.GenerateMesh();
    }
}
