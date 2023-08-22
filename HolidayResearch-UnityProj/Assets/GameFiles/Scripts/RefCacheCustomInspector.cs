using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class RefCacheCustomInspector : EditorWindow
{
    // Refactor this once the system is done

    // Create basic GUI, then research how to create better GUI
    // Add target lock

    private struct ReferenceCache // do not re-use this naming convention in the refactor
    {
        public ReferenceCache(SerializedObject _serializedObject)
        {
            serializedObject = _serializedObject;
            requiredReferences = new List<SerializedProperty>();
            nullableRequired = new List<SerializedProperty>();
            componentNullable = new List<SerializedProperty>();
        }

        public SerializedObject serializedObject;
        public List<SerializedProperty> requiredReferences;
        public List<SerializedProperty> nullableRequired;
        public List<SerializedProperty> componentNullable;
    }

    bool locked = false;
    GUIStyle lockButtonStyle;

    public const string windowTitle = "Reference Cache Inspector";

    List<ReferenceCache> selectedSerializedObjects = new List<ReferenceCache>();

    private void OnSelectionChange()
    {
        if (locked == true) { return; }

        ReSelect();
    }

    private void OnGUI()
    {
        foreach (ReferenceCache referenceCache in selectedSerializedObjects)
        { 
            //EditorGUILayout.LabelField(referenceCache.serializedObject.targetObject.GetType().Name);

            EditorGUILayout.InspectorTitlebar(true, referenceCache.serializedObject.targetObject);

            DrawRequiredReferences(referenceCache);
            DrawNullableRequired(referenceCache);
            DrawComponentNullable(referenceCache);

            EditorGUILayout.Space();
        }


        Rect endLine = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 1f);
        EditorGUI.DrawRect(endLine, Color.grey);
    }

    // Lock Button
    // http://leahayes.co.uk/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow.html
    // Magic method which Unity detects automatically.
    private void ShowButton(Rect position)
    {
        if (lockButtonStyle == null)
        {
            lockButtonStyle = "IN LockButton";
        }

        if (locked == false)
        {
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }
        else
        {
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
            if (locked == false)
            {
                ReSelect();
            }
        }
    }

    public static void Open(Object requestingObject, GameObject requestingHostObject = null)
    {
        RefCacheCustomInspector window = GetWindow<RefCacheCustomInspector>(windowTitle);

        if (requestingHostObject != null)
        {
            Selection.activeObject = requestingHostObject;
        }
        else
        {
            Selection.activeObject = requestingObject;
        }

        window.Repaint();

        if (window.HandleSelectedGameObjects(ref window.selectedSerializedObjects) == true)
        {
            return;
        }

        if (window.HandleSelectedObject(ref window.selectedSerializedObjects) == true)
        {
            return;
        }

        window.selectedSerializedObjects.Clear();
    }

    void ReSelect()
    {
        Repaint();

        if (HandleSelectedGameObjects(ref selectedSerializedObjects) == true)
        {
            return;
        }

        if (HandleSelectedObject(ref selectedSerializedObjects) == true)
        {
            return;
        }

        selectedSerializedObjects.Clear();
    }

    void DrawRequiredReferences(ReferenceCache referenceCache)
    {
        if (referenceCache.requiredReferences == null) { return; }
        if (referenceCache.requiredReferences.Count == 0) { return; }

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Required References");
        foreach (SerializedProperty serializedProperty in referenceCache.requiredReferences)
        {
            EditorGUILayout.PropertyField(serializedProperty);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void DrawNullableRequired(ReferenceCache referenceCache)
    {
        if (referenceCache.nullableRequired == null) { return; }
        if (referenceCache.nullableRequired.Count == 0) { return; }

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Nullable Required");
        foreach (SerializedProperty serializedProperty in referenceCache.nullableRequired)
        {
            EditorGUILayout.PropertyField(serializedProperty);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }


    void DrawComponentNullable(ReferenceCache referenceCache)
    {
        if (referenceCache.componentNullable == null) { return; }
        if (referenceCache.componentNullable.Count == 0) { return; }

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Component Nullable");
        foreach (SerializedProperty serializedProperty in referenceCache.componentNullable)
        {
            EditorGUILayout.PropertyField(serializedProperty);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    ReferenceCache GetAllReferences(ReferenceCache referenceCache)
    {
        referenceCache.requiredReferences = GetRequiredReferences(referenceCache.serializedObject);
        referenceCache.nullableRequired = GetNullableRequired(referenceCache.serializedObject);
        referenceCache.componentNullable = GetComponentNullable(referenceCache.serializedObject);
        return referenceCache;
    }

    List<ReferenceCache> GetAllReferences(List<ReferenceCache> references)
    {
        List<ReferenceCache> returnList = references;
        for(int loop = 0; loop < returnList.Count; loop++)
        {
            returnList[loop] = GetAllReferences(references[loop]);
        }
        return returnList;
    }

    List<SerializedProperty> GetRequiredReferences(SerializedObject serializedObject)
    {
        // Returns false if the output list has a count of 0 by the end of the operation

        List<SerializedProperty> returnList = new List<SerializedProperty>();

        System.Type type = serializedObject.targetObject.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = type.GetMembers(flags);

        foreach (MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length == 0) { continue; }

            RequiredReferenceAttribute requiredReferenceAtt = member.GetCustomAttribute<RequiredReferenceAttribute>();
            if (requiredReferenceAtt != null)
            {
                returnList.Add(serializedObject.FindProperty(member.Name));
            }
        }

        return returnList;
    }

    List<SerializedProperty> GetNullableRequired(SerializedObject serializedObject)
    {
        return null;
    }

    List<SerializedProperty> GetComponentNullable(SerializedObject serializedObject)
    {
        return null;
    }

    bool HandleSelectedObject(ref List<ReferenceCache> outputList, bool clearList = true) // use an object field instead of Selection.activeObject in the refactor
    {
        // Valid is defined as the object containing a field attributed with any of the following: [RequiredReference] [RequiredNullable] [ComponentNullable]
        // it will return false if no valid fields are found in the object

        if (Selection.activeObject == null) { return false; }
        Object selected = Selection.activeObject;

        if (clearList == true)
        {
            outputList.Clear();
        }

        bool validReferencesFound = ScanObjectForValidField(selected);
        if (validReferencesFound == false) { return false; }

        outputList = GetAllReferences(outputList);
        return true;
    }

    bool HandleSelectedGameObjects(ref List<ReferenceCache> outputList, bool clearList = true) // use an object field instead of Selection.activeGameObject in the refactor
    {
        // returns false if selection is null or no valid components are found

        if (Selection.activeGameObject == null) { return false; }
        GameObject selected = Selection.activeGameObject;

        List<MonoBehaviour> components = new List<MonoBehaviour>();
        selected.GetComponents(components);

        bool validComponentsFound = FillListWithValidComponents(components, ref outputList, clearList);

        if (validComponentsFound == false) { return false; }

        outputList = GetAllReferences(outputList);
        return true;
    }

    bool FillListWithValidComponents(List<MonoBehaviour> components, ref List<ReferenceCache> outputList, bool clearList = true)
    {
        // Valid is defined as the component containing a field attributed with any of the following: [RequiredReference] [RequiredNullable] [ComponentNullable]
        // it will return false if no valid components are found

        if (clearList == true)
        {
            outputList.Clear();
        }

        foreach (MonoBehaviour component in components)
        {
            if (ScanObjectForValidField(component) == true)
            {
                ReferenceCache newSerializedObject = new ReferenceCache(new SerializedObject(component));
                outputList.Add(newSerializedObject);
            }
        }

        if(outputList.Count == 0)
        {
            return false;
        }

        return true;
    }

    bool ScanObjectForValidField(Object component)
    {
        // Valid is defined as the component containing a field attributed with any of the following: [RequiredReference] [RequiredNullable] [ComponentNullable]
        // it will return false if no valid fields are found

        System.Type type = component.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = type.GetMembers(flags);

        foreach (MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length == 0) { continue; }

            RequiredReferenceAttribute requiredReferenceAtt = member.GetCustomAttribute<RequiredReferenceAttribute>();
            if(requiredReferenceAtt != null)
            {
                return true;
            }

            //RequiredNullableAttribute requiredNullableAtt = member.GetCustomAttribute<RequiredNullableAttribute>();
            //if (requiredNullableAtt != null)
            //{
            //    return true;
            //}

            //ComponentNullableAttribute componentNullableAtt = member.GetCustomAttribute<ComponentNullableAttribute>();
            //if (componentNullableAtt != null)
            //{
            //    return true;
            //}
        }

        return false;
    }
}
