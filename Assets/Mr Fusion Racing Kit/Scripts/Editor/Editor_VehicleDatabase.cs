using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

//[CustomEditor(typeof(VehicleDatabase))]
public class Editor_VehicleDatabase : Editor
{
    VehicleDatabase _target;

    SerializedProperty vehicleClasses;
    SerializedProperty vehicles;


    void OnEnable()
    {

    }


    public override void OnInspectorGUI()
    {

    }
}
