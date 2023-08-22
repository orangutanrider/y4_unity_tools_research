using UnityEditor;
using UnityEngine;


// This is entirely unused
[CustomPropertyDrawer(typeof(RequiredReferenceAttribute))]
public class InspectorEndRectDecoratorDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        return EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position)
    {
        Rect rect = position;
        rect.height = EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.DrawRect(rect, Color.grey);
    }
}
