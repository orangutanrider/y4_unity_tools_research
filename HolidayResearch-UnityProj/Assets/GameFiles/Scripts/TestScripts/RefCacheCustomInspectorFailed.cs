using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class RefCacheCustomInspectorFailed : EditorWindow
{
    // https://forum.unity.com/threads/solved-recreate-custom-inspector-within-custom-inspector.427519/
    // https://docs.unity3d.com/ScriptReference/Editor.html
    // hmm!?
    // Why is there such a tendancy to call the inspector an object editor?

    public const string windowTitle = "Reference Cache Inspector";

    SerializedObject selectedSerializedObject;
    Object selectedObject;

    bool requiredRefFoldout = true;
    List<SerializedProperty> selectedObjectRequiredRefs = new List<SerializedProperty>();
    // Required References must be declared in one of two ways.
    // public [HideInInspector, RequiredReference]
    // [SerializeField, HideInInspector, RequiredReference]
    // If you wish to have the field appear both in the inspector and the custom inspector, then remove [HideInInspector]

    private void OnSelectionChange()
    {
        selectedObject = Selection.activeObject;
        selectedSerializedObject = new SerializedObject(selectedObject);
        FillRequiredReferenceList(selectedSerializedObject, ref selectedObjectRequiredRefs);
    }

    private void OnGUI()
    {
        DrawRequiredReferences();
    }

    public static void Open(Object requestingObject)
    {
        RefCacheCustomInspectorFailed inspectorWindow = GetWindow<RefCacheCustomInspectorFailed>(windowTitle);

        Selection.activeObject = requestingObject;
        inspectorWindow.selectedObject = requestingObject;
        inspectorWindow.selectedSerializedObject = new SerializedObject(inspectorWindow.selectedObject);
        inspectorWindow.FillRequiredReferenceList(inspectorWindow.selectedSerializedObject, ref inspectorWindow.selectedObjectRequiredRefs);
    }

    void DrawRequiredReferences()
    {
        if (selectedObjectRequiredRefs.Count == 0)
        {
            return;
        }

        requiredRefFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(requiredRefFoldout, "Required References");
        if (requiredRefFoldout)
        {
            foreach (SerializedProperty serializedProperty in selectedObjectRequiredRefs)
            {
                EditorGUILayout.PropertyField(serializedProperty);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void DisplayNothingToInspect()
    {
        // Nothing inspectable by this editor was found
    }

    void FillRequiredReferenceList(SerializedObject serializedObject, ref List<SerializedProperty> list)
    {
        list.Clear();

        System.Type type = serializedObject.targetObject.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = type.GetMembers(flags);

        foreach (MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length == 0) { continue; }

            RequiredReferenceAttribute attribute = member.GetCustomAttribute<RequiredReferenceAttribute>();
            if (attribute == null) { continue; }

            list.Add(serializedObject.FindProperty(member.Name));
        }
    }
}
