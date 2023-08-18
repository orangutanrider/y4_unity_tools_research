using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoTestRequiredReference)), CanEditMultipleObjects]
public class MonoTestRequiredReferenceCustomEditor : Editor
{
    SerializedProperty requiredReferenceGameobject;
    SerializedProperty requiredReferenceMonobehaviour;
    SerializedProperty requiredReferenceScriptableObject;

    bool requiredReferencesGroup = false;

    private void OnEnable()
    {
        requiredReferenceGameobject = serializedObject.FindProperty("requiredReferenceGameobject");
        requiredReferenceMonobehaviour = serializedObject.FindProperty("requiredReferenceMonobehaviour");
        requiredReferenceScriptableObject = serializedObject.FindProperty("requiredReferenceScriptableObject");
    }

    public override void OnInspectorGUI()
    {
        MonoTestRequiredReference testMonobehaviour = (MonoTestRequiredReference)target;

        requiredReferencesGroup = EditorGUILayout.BeginFoldoutHeaderGroup(requiredReferencesGroup, "Required References");
        if (requiredReferencesGroup)
        {
            EditorGUILayout.PropertyField(requiredReferenceGameobject);
            EditorGUILayout.PropertyField(requiredReferenceMonobehaviour);
            EditorGUILayout.PropertyField(requiredReferenceScriptableObject);
            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        #region Display Error 
        bool isThereNullReference = false;
        if (testMonobehaviour.requiredReferenceGameobject == null)
        {
            isThereNullReference = true;
        }
        if (testMonobehaviour.requiredReferenceMonobehaviour == null)
        {
            isThereNullReference = true;
        }
        if (testMonobehaviour.requiredReferenceScriptableObject == null)
        {
            isThereNullReference = true;
        }

        if (isThereNullReference == true)
        {
            EditorGUILayout.HelpBox("There are NULL RequiredReferences", MessageType.Error);
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
