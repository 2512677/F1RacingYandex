using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;


[CustomEditor(typeof(TrackCamera))]
public class Editor_TrackCamera : Editor
{
    TrackCamera _target;

    void OnEnable()
    {
        _target = (TrackCamera)target;
    }


    public override void OnInspectorGUI()
    {
        bool isPreview = _target.gameObject.GetComponent<Camera>();

        if (GUILayout.Button(!isPreview ? "Предварительный просмотр" : "Удалить предварительный просмотр"))
        {
            //Create Camera
            if (!isPreview)
            {
                _target.gameObject.AddComponent<Camera>();
            }
            
            //Destroy Camera
            if (isPreview)
            {
                DestroyImmediate(_target.gameObject.GetComponent<Camera>());
                EditorGUIUtility.ExitGUI();
            }
        }
    }
}
