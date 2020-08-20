using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeMeshCreator))]
public class PipeMeshCreatorEditor : Editor 
{
    PipeMeshCreator pipeMeshCreator;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Create Mesh"))
        {
            pipeMeshCreator = (PipeMeshCreator)target;
            pipeMeshCreator.CreateMesh();
        }
    }

    public void OnValidate()
    {
        if(pipeMeshCreator == null)
            pipeMeshCreator = (PipeMeshCreator)target;
    }
}
