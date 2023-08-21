using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

public class RequiredReferenceFailedDrawer1 : PropertyDrawer
{
    // this doesn't work because you simply can't use GUI methods to draw things outside of the OnGUI method.
    // this script is also un-finished though, the finished version would've had a system for offsetting all the fields so that the foldout header can fit in
    // as it was also discovered that EditorGUILayout methods don't work with OnGUI either (so positioning and the sorts would have to be done manually)
    
    private struct RequiredRef
    {
        public RequiredRef(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            position = _position;
            property = _property;
            label = _label;
        }

        public Rect position;
        public SerializedProperty property;
        public GUIContent label;
    }

    private class RequiredRefFoldoutGroup
    {
        public RequiredRefFoldoutGroup(Type hostType, RequiredRef requiredRef)
        {
            CalculateExpectedTotalRequiredRefs(hostType);
            foldout = EditorGUI.BeginFoldoutHeaderGroup(requiredRef.position, foldout, requiredRef.label);
        }

        bool foldout = false;
        int onGUIRequests = 0;
        int totalOnGUIExpected = 0;

        public void ProcessNewOnGUI(RequiredRef requiredRef)
        {
            foldoutGroup.onGUIRequests++;

            if (AllGUIRequested == false) { return; }

            onGUIRequests = 0;

            EditorGUI.EndFoldoutHeaderGroup();
        }

        void CalculateExpectedTotalRequiredRefs(Type hostType)
        {
            totalOnGUIExpected = 0;

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] members = hostType.GetMembers(flags);

            foreach (MemberInfo member in members)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                RequiredReferenceAttribute attribute = member.GetCustomAttribute<RequiredReferenceAttribute>();
                if (attribute == null) { continue; }

                totalOnGUIExpected++;
            }
        }

        bool AllGUIRequested
        {
            get
            {
                if (onGUIRequests >= totalOnGUIExpected)
                {
                    return true;
                }
                return false;
            }
        }
    }

    static RequiredRefFoldoutGroup foldoutGroup = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // base.OnGUI(position, property, label);
        // EditorGUI.PropertyField(position, property, label, true);

        if (foldoutGroup == null)
        {
            Type hostType = property.serializedObject.targetObject.GetType();
            RequiredRef newRequiredRef1 = new RequiredRef(position, property, label);

            foldoutGroup = new RequiredRefFoldoutGroup(hostType, newRequiredRef1);

            EditorGUI.PropertyField(position, property, label, true);

            foldoutGroup.ProcessNewOnGUI(newRequiredRef1);
            return;
        }

        EditorGUI.PropertyField(position, property, label, true);

        RequiredRef newRequiredRef2 = new RequiredRef(position, property, label);
        foldoutGroup.ProcessNewOnGUI(newRequiredRef2);
        return;
    }
}
