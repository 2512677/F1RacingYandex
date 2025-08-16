using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

//[CustomEditor(typeof(VehicleData))]
public class Editor_VehicleData : Editor
{
    VehicleData2 _target;

    //Editor vehiclePreview;

    void OnEnable()
    {
        _target = (VehicleData2)target;

        if (_target.vehicle != null)
        {
            //vehiclePreview = Editor.CreateEditor(_target.vehicle);
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        _target.vehicle = EditorGUILayout.ObjectField("Vehicle Prefab", _target.vehicle, typeof(GameObject), true) as GameObject;
        _target.vehicleName = EditorGUILayout.TextField("Name", _target.vehicleName);
        //_target.unlocked = EditorGUILayout.Toggle("Is Unlocked", _target.unlocked);
        //_target.togglePreview = EditorGUILayout.Toggle("Toggle Preview", _target.togglePreview);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Description:");
        _target.vehicleDescription = EditorGUILayout.TextArea(_target.vehicleDescription, EditorStyles.textArea, GUILayout.Height(50));

        //if (vehiclePreview != null && _target.togglePreview)
        //{
            //EditorGUILayout.LabelField("Preview:");
            //vehiclePreview.OnPreviewGUI(GUILayoutUtility.GetRect(100, 300), EditorStyles.helpBox);
        //}

        GUILayout.EndVertical();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(_target);
        }
    }
}