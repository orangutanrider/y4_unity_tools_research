using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HelpMessageTypeStorage))]
public class HelpMessageTypeStorageCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HelpMessageTypeStorage typeStorage = (HelpMessageTypeStorage)target;

        EditorGUILayout.LabelField(typeStorage.assembliesList.Count.ToString());

        if (GUILayout.Button("GetAssemblies"))
        {
            typeStorage.GetAssemblies();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(typeStorage.methodList.Count.ToString());
        if (GUILayout.Button("GetMethods"))
        {
            typeStorage.GetInterfacesButton();
        }


        serializedObject.ApplyModifiedProperties();
    }
}
