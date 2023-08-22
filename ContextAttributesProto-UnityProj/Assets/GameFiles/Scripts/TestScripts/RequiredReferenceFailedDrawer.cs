using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

//[CustomPropertyDrawer(typeof(RequiredReferenceAttribute))]
public class RequiredReferenceFailedDrawer : PropertyDrawer
{
    private class RequiredRefGroup
    {
        public RequiredRefGroup(SerializedObject _serializedObject)
        {
            serializedObject = _serializedObject;
        }

        public SerializedObject serializedObject;

        public bool foldout = false;
        public List<RequiredRef> requiredRefs = new List<RequiredRef>();
    }

    private class RequiredRef
    {
        public RequiredRef(SerializedProperty _serializedProperty)
        {
            serializedProperty = _serializedProperty;
        }

        public SerializedProperty serializedProperty;
        public bool guiDrawn = false;
    }

    static List<RequiredRefGroup> requiredRefGroups = new List<RequiredRefGroup>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RequiredRefGroup group = RegisterSerializedProperty(property, ref requiredRefGroups);
        RequiredRef requiredRefBeingDrawn = GetRequiredRef(property, group);

        if (requiredRefBeingDrawn == null) { return; }

        requiredRefBeingDrawn.guiDrawn = true;

        // Check if all RequiredRefs have recieved a draw call
        foreach (RequiredRef requiredRef in group.requiredRefs)
        {
            if (requiredRef.guiDrawn == false)
            {
                return;
            }
        }

        // If all of them have, then draw the group
        DrawGroup(group);
    }

    void DrawGroup(RequiredRefGroup group)
    {
        // Reset draw calls for next time
        foreach (RequiredRef requiredRef in group.requiredRefs)
        {
            requiredRef.guiDrawn = false;
        }

        // Fields Dropdown
        group.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(group.foldout, "Required References");
        if (group.foldout)
        {
            foreach (RequiredRef requiredRef in group.requiredRefs)
            {
                EditorGUILayout.PropertyField(requiredRef.serializedProperty);
            }
            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Display Error HelpBox
        bool isThereNullReference = false;
        foreach (RequiredRef requiredRef in group.requiredRefs)
        {
            if (requiredRef.serializedProperty.objectReferenceValue == null)
            {
                isThereNullReference = true;
            }
        }
        if (isThereNullReference == true)
        {
            EditorGUILayout.HelpBox("There are NULL RequiredReferences", MessageType.Error);
        }
    }

    RequiredRef GetRequiredRef(SerializedProperty serializedProperty, RequiredRefGroup searchGroup)
    {
        foreach (RequiredRef requiredRef in searchGroup.requiredRefs)
        {
            if (serializedProperty.name == requiredRef.serializedProperty.name)
            {
                return requiredRef;
            }
        }
        return null;
    }

    RequiredRefGroup RegisterSerializedProperty(SerializedProperty serializedProperty, ref List<RequiredRefGroup> groups)
    {
        // If a matching serializedObject is found, return as it is expected that this reqRef was already added to that group
        foreach (RequiredRefGroup requiredRefGroup in groups)
        {
            if (requiredRefGroup.serializedObject == serializedProperty.serializedObject)
            {
                return requiredRefGroup;
            }
        }

        RequiredRefGroup newRequiredRefGroup = new RequiredRefGroup(serializedProperty.serializedObject);
        groups.Add(newRequiredRefGroup);

        FillListWithRequiredRefsInType(serializedProperty.serializedObject.targetObject.GetType(), serializedProperty.serializedObject, ref newRequiredRefGroup.requiredRefs);
        return newRequiredRefGroup;
    }

    void FillListWithRequiredRefsInType(Type t, SerializedObject serializedObject, ref List<RequiredRef> list, bool debugMode = false)
    {
        int totalAdded = 0;

        list.Clear();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = t.GetMembers(flags);

        foreach (MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length == 0) { continue; }

            RequiredReferenceAttribute attribute = member.GetCustomAttribute<RequiredReferenceAttribute>();
            if (attribute == null) { continue; }

            RequiredRef newRequiredRef = new RequiredRef(serializedObject.FindProperty(member.Name));
            list.Add(newRequiredRef);

            totalAdded++;
        }

        if (debugMode == true) // Update in future, make it formatted nicely via the debug message library and make use of IDebugMode
        {
            Debug.Log(t);
            Debug.Log(totalAdded);
        }
    }
}
