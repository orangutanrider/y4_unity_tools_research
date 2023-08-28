using UnityEditor;
using System.Collections.Generic;

public class SceneAssetRefsObj
{
    public SceneAssetRefsObj(SerializedObject _serializedObject)
    {
        TitlebarFoldout = true;
        serializedObject = _serializedObject;

        RequiredReferencesFoldout = true;
        requiredReferences = new List<SerializedProperty>();
        inheritedRequiredReferences = new List<InheritedSerializedProperties>();

        NullableRequiredFoldout = true;
        nullableRequired = new List<SerializedProperty>();
        inheritedNullableRequired = new List<InheritedSerializedProperties>();

        ComponentNullableFoldout = true;
        componentNullable = new List<SerializedProperty>();
        inheritedComponentNullable = new List<InheritedSerializedProperties>();
    }

    public bool TitlebarFoldout { get; set; }
    public SerializedObject serializedObject;

    public bool RequiredReferencesFoldout { get; set; }
    public List<SerializedProperty> requiredReferences;
    public List<InheritedSerializedProperties> inheritedRequiredReferences;

    public bool NullableRequiredFoldout { get; set; }
    public List<SerializedProperty> nullableRequired;
    public List<InheritedSerializedProperties> inheritedNullableRequired;

    public bool ComponentNullableFoldout { get; set; }
    public List<SerializedProperty> componentNullable;
    public List<InheritedSerializedProperties> inheritedComponentNullable;
}
