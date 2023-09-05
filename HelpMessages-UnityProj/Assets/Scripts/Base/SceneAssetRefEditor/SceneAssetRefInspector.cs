using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using EditorUtilities.AttributedSerializedExtensions;

[EditorWindowTitle(icon = "Assets/GameFiles/Scripts/SceneAssetRefEditor/Editor/SceneAssetRefInspectorIcon.png", title = " Scene/Asset Ref Inspector")]
public class SceneAssetRefInspector : CustomInspector
{
    static readonly System.Type[] allAttributeTypes = new System.Type[] { typeof(RequiredReferenceAttribute), typeof(NullableRequiredAttribute), typeof(ComponentNullableAttribute) };

    List<SceneAssetRefsObj> sceneAssetRefsObjs = new List<SceneAssetRefsObj>();

    public const string readMe = "Bla, bla bla bla, bla. Bla, bla, bla, BLa, bla,bal, aslb dblal balsa bldsal bldas, Pooter, hooter, footer, looter, pootis.";

    static readonly HelpMessageData requiredReferenceMsg = new HelpMessageData("There are NULL Required References.", MessageType.Error);
    static readonly HelpMessageData nullableRequiredMsg = new HelpMessageData("There are NULL Nullable Required.", MessageType.Warning);
    static readonly HelpMessageData componentNullableMsg = new HelpMessageData("There are NULL Component Nullable.", MessageType.Info);

    private void OnGUI()
    {
        bool changesApplied = false;

        StartGUIContent();

        // Content
        foreach (SceneAssetRefsObj sceneAssetObj in sceneAssetRefsObjs)
        {
            DrawSceneAssetRefsObj(sceneAssetObj);
            if (sceneAssetObj.serializedObject.ApplyModifiedProperties() == true)
            {
                changesApplied = true;
            }
        }
        DrawInspectorEndLine();

        EndGUIContent();

        if(changesApplied == true)
        {
            HelpMessageInspector.RequestMessageUpdate();
        }
    }

    protected override void SelectionUpdate(Object newSelectedObject, GameObject newSelectedGameObject)
    {
        if(TryGetSceneAssetRefs(newSelectedGameObject, ref sceneAssetRefsObjs) == true)
        {
            return;
        }

        if (TryGetSceneAssetRefs(newSelectedObject, ref sceneAssetRefsObjs) == true)
        {
            return;
        }

        sceneAssetRefsObjs.Clear();
    }

    [MenuItem("CustomInspectors/SceneAsset Ref Inspector")]
    public static void OpenWindowRelay()
    {
        Open(typeof(SceneAssetRefInspector));
    }

    [HelpMessageEditorProvider] // could possibly do it based on method name, rather than attribute
    public static List<HelpMessageData> SendHelpMessages(SerializedObject serializedObject)
    {
        List<HelpMessageData> returnList = new List<HelpMessageData>();

        List<SceneAssetRefsObj> sceneAssetRefs = new List<SceneAssetRefsObj>();
        if(TryGetSceneAssetRefs(serializedObject.targetObject, ref sceneAssetRefs) == false) { return returnList; }

        foreach (SceneAssetRefsObj sceneAssetRef in sceneAssetRefs) 
        {
            if (DoesRequiredReferenceMsgShow(sceneAssetRef) == true)
            {
                returnList.Add(new HelpMessageData(requiredReferenceMsg));
            }

            if (DoesNullableRequiredMsgShow(sceneAssetRef) == true)
            {
                returnList.Add(new HelpMessageData(nullableRequiredMsg));
            }

            if (DoesComponentNullableMsgShow(sceneAssetRef) == true)
            {
                returnList.Add(new HelpMessageData(componentNullableMsg));
            }
        }

        return returnList;
    }

    #region GUI Drawing
    void DrawSceneAssetRefsObj(SceneAssetRefsObj sceneAssetObj)
    {
        sceneAssetObj.TitlebarFoldout = EditorGUILayout.InspectorTitlebar(sceneAssetObj.TitlebarFoldout, sceneAssetObj.serializedObject.targetObject);

        if (sceneAssetObj.TitlebarFoldout == false) 
        {
            return;
        }

        DrawMonoBehaviourScriptField(sceneAssetObj.serializedObject.targetObject, sceneAssetObj.serializedObject.targetObject.GetType());

        if (IsAnyRequiredReferencePresent(sceneAssetObj) == true)
        {
            sceneAssetObj.RequiredReferencesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(sceneAssetObj.RequiredReferencesFoldout, "Required Reference");
            if (sceneAssetObj.RequiredReferencesFoldout)
            {
                DrawRequiredReference(sceneAssetObj);
                DrawInheritedRequiredReference(sceneAssetObj);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (IsAnyNullableRequiredPresent(sceneAssetObj) == true)
        {
            sceneAssetObj.NullableRequiredFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(sceneAssetObj.NullableRequiredFoldout, "Nullable Required");
            if (sceneAssetObj.NullableRequiredFoldout)
            {
                DrawNullableRequired(sceneAssetObj);
                DrawInheritedNullableRequired(sceneAssetObj);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (IsAnyComponentNullablePresent(sceneAssetObj) == true)
        {
            sceneAssetObj.ComponentNullableFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(sceneAssetObj.ComponentNullableFoldout, "Component Nullable");
            if (sceneAssetObj.ComponentNullableFoldout)
            {
                DrawComponentNullable(sceneAssetObj);
                DrawInheritedComponentNullable(sceneAssetObj);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUILayout.Space();
    }

    // Required References
    void DrawRequiredReference(SceneAssetRefsObj sceneAssetObj)
    {
        if(sceneAssetObj.requiredReferences == null) { return; }
        if(sceneAssetObj.requiredReferences.Count == 0) { return; }

        foreach(SerializedProperty serializedProperty in sceneAssetObj.requiredReferences)
        {
            EditorGUILayout.PropertyField(serializedProperty );
        }
    }
    void DrawInheritedRequiredReference(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.inheritedRequiredReferences == null) { return; }
        if (sceneAssetObj.inheritedRequiredReferences.Count == 0) { return; }

        foreach (InheritedSerializedProperties inheritedProperties in sceneAssetObj.inheritedRequiredReferences)
        {
            foreach (SerializedProperty serializedProperty in inheritedProperties.properties)
            {
                GUIContent label = new GUIContent("(" + inheritedProperties.type.Name + ") " + serializedProperty.displayName);
                EditorGUILayout.PropertyField(serializedProperty, label);
            }
        }
    }

    // Nullable Required
    void DrawNullableRequired(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.nullableRequired == null) { return; }
        if (sceneAssetObj.nullableRequired.Count == 0) { return; }

        foreach (SerializedProperty serializedProperty in sceneAssetObj.nullableRequired)
        {
            EditorGUILayout.PropertyField(serializedProperty);
        }
    }
    void DrawInheritedNullableRequired(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.inheritedNullableRequired == null) { return; }
        if (sceneAssetObj.inheritedNullableRequired.Count == 0) { return; }

        foreach (InheritedSerializedProperties inheritedProperties in sceneAssetObj.inheritedNullableRequired)
        {
            foreach (SerializedProperty serializedProperty in inheritedProperties.properties)
            {
                GUIContent label = new GUIContent("(" + inheritedProperties.type.Name + ") " + serializedProperty.displayName);
                EditorGUILayout.PropertyField(serializedProperty, label);
            }
        }
    }

    // Component Nullable
    void DrawComponentNullable(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.componentNullable == null) { return; }
        if (sceneAssetObj.componentNullable.Count == 0) { return; }

        foreach (SerializedProperty serializedProperty in sceneAssetObj.componentNullable)
        {
            EditorGUILayout.PropertyField(serializedProperty);
        }
    }
    void DrawInheritedComponentNullable(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.inheritedComponentNullable == null) { return; }
        if (sceneAssetObj.inheritedComponentNullable.Count == 0) { return; }

        foreach (InheritedSerializedProperties inheritedProperties in sceneAssetObj.inheritedComponentNullable)
        {
            foreach (SerializedProperty serializedProperty in inheritedProperties.properties)
            {
                GUIContent label = new GUIContent("(" + inheritedProperties.type.Name + ") " + serializedProperty.displayName);
                EditorGUILayout.PropertyField(serializedProperty, label);
            }
        }
    }
    #endregion

    #region Data Processing
    // Required Reference
    static bool IsAnyRequiredReferencePresent(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.requiredReferences == null && sceneAssetObj.inheritedRequiredReferences == null) { return false; }
        if (sceneAssetObj.requiredReferences.Count == 0 && sceneAssetObj.inheritedRequiredReferences.Count == 0) { return false; }
        return true;
    }

    static bool DoesRequiredReferenceMsgShow(SceneAssetRefsObj sceneAssetObj)
    {
        if (IsAnyRequiredReferencePresent(sceneAssetObj) == false) { return false; }

        foreach (SerializedProperty serializedProperty in sceneAssetObj.requiredReferences)
        {
            if (serializedProperty == null) { continue; }

            if (serializedProperty.objectReferenceValue == null)
            {
                return true;
            }
        }

        foreach (InheritedSerializedProperties inherited in sceneAssetObj.inheritedRequiredReferences)
        {
            foreach (SerializedProperty serializedProperty in sceneAssetObj.requiredReferences)
            {
                if (serializedProperty == null) { continue; }

                if (serializedProperty.objectReferenceValue == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Nullable Required
    static bool IsAnyNullableRequiredPresent(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.nullableRequired == null && sceneAssetObj.inheritedNullableRequired == null) { return false; }
        if (sceneAssetObj.nullableRequired.Count == 0 && sceneAssetObj.inheritedNullableRequired.Count == 0) { return false; }
        return true;
    }

    static bool DoesNullableRequiredMsgShow(SceneAssetRefsObj sceneAssetObj)
    {
        if (IsAnyNullableRequiredPresent(sceneAssetObj) == false) { return false; }

        foreach (SerializedProperty serializedProperty in sceneAssetObj.nullableRequired)
        {
            if (serializedProperty == null) { continue; }

            if (serializedProperty.objectReferenceValue == null)
            {
                return true;
            }
        }

        foreach (InheritedSerializedProperties inherited in sceneAssetObj.inheritedNullableRequired)
        {
            foreach (SerializedProperty serializedProperty in sceneAssetObj.nullableRequired)
            {
                if (serializedProperty == null) { continue; }

                if (serializedProperty.objectReferenceValue == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Component Nullable
    static bool IsAnyComponentNullablePresent(SceneAssetRefsObj sceneAssetObj)
    {
        if (sceneAssetObj.componentNullable == null && sceneAssetObj.inheritedComponentNullable == null) { return false; }
        if (sceneAssetObj.componentNullable.Count == 0 && sceneAssetObj.inheritedComponentNullable.Count == 0) { return false; }
        return true;
    }

    static bool DoesComponentNullableMsgShow(SceneAssetRefsObj sceneAssetObj)
    {
        if (IsAnyComponentNullablePresent(sceneAssetObj) == false) { return false; }

        foreach (SerializedProperty serializedProperty in sceneAssetObj.componentNullable)
        {
            if (serializedProperty == null) { continue; }

            if (serializedProperty.objectReferenceValue == null)
            {
                return true;
            }
        }

        foreach (InheritedSerializedProperties inherited in sceneAssetObj.inheritedComponentNullable)
        {
            foreach (SerializedProperty serializedProperty in sceneAssetObj.componentNullable)
            {
                if (serializedProperty == null) { continue; }

                if (serializedProperty.objectReferenceValue == null)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region SceneAssetRefsObj Generation
    static bool TryGetSceneAssetRefs(GameObject gameObject, ref List<SceneAssetRefsObj> sceneAssetObjs)
    {
        if (gameObject == null) { return false; }

        List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
        gameObject.GetComponents(monoBehaviours);

        List<SerializedObject> serializedObjectOutput = new List<SerializedObject>();
        if (allAttributeTypes.TryGetSerializedObjectsContainingAttributeTypes(monoBehaviours, ref serializedObjectOutput) == false) { return false; }

        sceneAssetObjs.Clear();
        foreach (SerializedObject serializedObject in serializedObjectOutput)
        {
            sceneAssetObjs.Add(new SceneAssetRefsObj(serializedObject));
        }

        sceneAssetObjs = CompleteSceneAssetObjs(sceneAssetObjs);

        return true;
    }

    static bool TryGetSceneAssetRefs(Object obj, ref List<SceneAssetRefsObj> sceneAssetObjs)
    {
        if (obj == null) { return false; }

        if (obj.DoesObjectContainAttributesOfType(allAttributeTypes) == false) { return false; }

        sceneAssetObjs.Clear();
        sceneAssetObjs.Add(new SceneAssetRefsObj(new SerializedObject(obj)));

        sceneAssetObjs = CompleteSceneAssetObjs(sceneAssetObjs);

        return true;
    }

    static List<SceneAssetRefsObj> CompleteSceneAssetObjs(List<SceneAssetRefsObj> sceneAssetObjs)
    {
        List<SceneAssetRefsObj> returnList = new List<SceneAssetRefsObj>();
        foreach(SceneAssetRefsObj sceneAssetObj in sceneAssetObjs)
        {
            returnList.Add(CompleteSceneAssetObj(sceneAssetObj));
        }
        return returnList;
    }

    static SceneAssetRefsObj CompleteSceneAssetObj(SceneAssetRefsObj sceneAssetObj)
    {
        MemberInfo[] memberInfo = sceneAssetObj.serializedObject.GetMembers();

        // Get RequiredReferences
        sceneAssetObj.requiredReferences = typeof(RequiredReferenceAttribute).GetPropertiesAttributedWithType(sceneAssetObj.serializedObject, memberInfo);
        sceneAssetObj.inheritedRequiredReferences = typeof(RequiredReferenceAttribute).GetInheritedPropertiesAttributedWithType(sceneAssetObj.serializedObject);

        // Get NullableRequired
        sceneAssetObj.nullableRequired = typeof(NullableRequiredAttribute).GetPropertiesAttributedWithType(sceneAssetObj.serializedObject, memberInfo);
        sceneAssetObj.inheritedNullableRequired = typeof(NullableRequiredAttribute).GetInheritedPropertiesAttributedWithType(sceneAssetObj.serializedObject);

        // Get ComponentNullable
        sceneAssetObj.componentNullable = typeof(ComponentNullableAttribute).GetPropertiesAttributedWithType(sceneAssetObj.serializedObject, memberInfo);
        sceneAssetObj.inheritedComponentNullable = typeof(ComponentNullableAttribute).GetInheritedPropertiesAttributedWithType(sceneAssetObj.serializedObject);

        return sceneAssetObj;
    }
    #endregion
}