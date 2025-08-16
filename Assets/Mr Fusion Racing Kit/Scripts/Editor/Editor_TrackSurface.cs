using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(TrackSurface))]
public class Editor_TrackSurface : Editor 
{
    TrackSurface m_target;

    void OnEnable()
    {
        m_target = (TrackSurface)target;
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Create New Surface Skidmarks"))
        {
            GameObject skidmarks = new GameObject("New Skidmarks");
            Undo.RegisterCreatedObjectUndo(skidmarks, "Created New Skidmarks");

            skidmarks.AddComponent<Skidmarks>();
            skidmarks.transform.SetParent(m_target.transform);

            Selection.activeGameObject = skidmarks;
        }
    }
}
