using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NullableRequiredAttribute))] // FIX NAMING MISS MATCH IN REFACTOR
public class RequiredNullableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            PropertyAlertGUI.ManualDrawAlertProperty(position, property, label, PropertyAlertGUI.AlertType.Warning);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
