using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubeMeshGenerator))]
public class CubeMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var currentTarget = (CubeMeshGenerator)target;

        currentTarget.size = EditorGUILayout.Slider("Size", currentTarget.size, 0.1f, 100f);
        currentTarget.resolution = EditorGUILayout.IntSlider("Resolution", currentTarget.resolution, 1, 255);

        if (GUILayout.Button("Generate") || GUI.changed)
            currentTarget.GenerateMesh();
    }
}
