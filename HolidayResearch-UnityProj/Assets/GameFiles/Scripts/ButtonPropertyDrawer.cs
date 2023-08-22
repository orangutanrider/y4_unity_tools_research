using UnityEditor;
using UnityEngine;

public abstract class ButtonPropertyDrawer : PropertyDrawer
{
    //https://forum.unity.com/threads/attribute-to-add-button-to-class.660262/

    protected abstract string ButtonName { get; }

    protected abstract void ButtonExecutionContainer(SerializedProperty property, GUIContent label);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // The button
        Rect buttonRect = position;
        buttonRect.height = EditorGUIUtility.singleLineHeight;
        if (GUI.Button(buttonRect, ButtonName))
        {
            ButtonExecutionContainer(property, label);
        }

        // The field that the button attribute is latched onto
        position.y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(position, property, label, true);
    }
}
