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

    bool foldOutShow = true;
    private void OnEnable()
    {
        requiredReferenceGameobject = serializedObject.FindProperty("requiredReferenceGameobject");
        requiredReferenceMonobehaviour = serializedObject.FindProperty("requiredReferenceMonobehaviour");
        requiredReferenceScriptableObject = serializedObject.FindProperty("requiredReferenceScriptableObject");
    }

    public override void OnInspectorGUI()
    {
        MonoTestRequiredReference testMonobehaviour = (MonoTestRequiredReference)target;

        EditorGUILayout.BeginFadeGroup(1f);
        EditorGUILayout.PropertyField(requiredReferenceGameobject);
        EditorGUILayout.PropertyField(requiredReferenceMonobehaviour);
        EditorGUILayout.PropertyField(requiredReferenceScriptableObject);
        EditorGUILayout.EndFadeGroup();

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
