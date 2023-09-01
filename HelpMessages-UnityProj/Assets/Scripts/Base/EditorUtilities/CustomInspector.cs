using UnityEditor;
using UnityEngine;

// Use with using EditorUtillities.AttributedSerializedExtensions; to gain access to AttributedSerializedExtensions.
public abstract class CustomInspector : EditorWindow
{
    protected bool Locked { get; set; } = false;
    GUIStyle lockButtonStyle;

    Vector2 scrollPosition = Vector2.zero;
    const int scrollWheelMultiply = 4;

    // This exists because there is a weird thing with the vertical group's return value. Every other call, it returns a height of 0, then the next call it returns the actual height of the content.
    // Don't know why or what is happening there, but I fixed by storing the valid values here, and if the height is 0, it uses this instead of the height gotten on that call.
    Rect previousContentRect = Rect.zero;
    int repaintBuffer = 1;

    const int scrollBarWidth = 13;

    private void Awake()
    {
        minSize = new Vector2(225, 100);
    }
    
    protected abstract void SelectionUpdate(Object newSelectedObject, GameObject newSelectedGameObject);

    private void OnSelectionChange()
    {
        if (Locked == true) { return; }
        SelectionUpdate(Selection.activeObject, Selection.activeGameObject);
        repaintBuffer = 0;
        Repaint();
    }

    // Window Lock Button
    // http://leahayes.co.uk/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow.html
    // It's a magic method which Unity detects automatically.
    private void ShowButton(Rect position)
    {
        if (lockButtonStyle == null)
        {
            lockButtonStyle = "IN LockButton";
        }

        if (Locked == false)
        {
            Locked = GUI.Toggle(position, Locked, GUIContent.none, lockButtonStyle);
        }
        else
        {
            Locked = GUI.Toggle(position, Locked, GUIContent.none, lockButtonStyle);
            if (Locked == false)
            {
                SelectionUpdate(Selection.activeObject, Selection.activeGameObject);
                repaintBuffer = 0;
                Repaint();
            }
        }
    }

    public static void Open(System.Type customInspectorType, Object newSelectedObject = null, GameObject newSelectedGameObject = null)
    {
        CustomInspector window = (CustomInspector)GetWindow(customInspectorType);
        if(window == null)
        {
            Debug.LogError("Invalid window type, it must inherit from type CustomInspector.");
            return;
        }

        window.Locked = false;
        window.SelectionUpdate(newSelectedObject, newSelectedGameObject);
        window.Repaint();
    }

    #region GUI Tools
    protected void DrawInspectorEndLine()
    {
        Rect endLineRect = GUILayoutUtility.GetRect(position.width - 1, 1);
        EditorGUI.DrawRect(endLineRect, Color.grey);
    }

    protected static void DrawMonoBehaviourScriptField(Object obj, System.Type type)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script ", obj, type, true);
        EditorGUI.EndDisabledGroup();
    }

    protected void StartGUIContent()
    {
        // Process scroll wheel
        if (Event.current.type == EventType.ScrollWheel)
        {
            scrollPosition.y = scrollPosition.y + (Event.current.delta.y * scrollWheelMultiply);
        }

        // This stops visual inconsistencies with foldout groups 
        if (Event.current.type == EventType.MouseUp)
        {
            repaintBuffer = 0;
        }

        // Begin view/area groups
        Rect areaRect = AreaRect(previousContentRect.height > position.height); // Get Rect
        GUI.BeginScrollView(ScrollPositionRect, scrollPosition, areaRect, false, false, GUIStyle.none, GUIStyle.none);
        GUILayout.BeginArea(areaRect);

        // Begin vertical group, to get the rect of the contents
        Rect contentRect = EditorGUILayout.BeginVertical();
        if (contentRect.height != 0) 
        { 
            previousContentRect = contentRect; 
        }
        else if(repaintBuffer == 0)
        {
            repaintBuffer++;
            Repaint();
        }
    }

    protected void EndGUIContent()
    {
        // End vertical group
        EditorGUILayout.EndVertical();
        
        // End view/area groups
        GUILayout.EndArea();
        GUI.EndScrollView();

        if (previousContentRect.height <= position.height) { return; }
            
        scrollPosition.y = GUI.VerticalScrollbar(ScrollBarRect, scrollPosition.y, position.height, 0, previousContentRect.height);

        // Took me like 3 days to make the scrollbar work how I wanted it to *-*
        // I was pulling my hair out over it
    }

    Rect ScrollBarRect
    {
        get { return new Rect(position.width - scrollBarWidth, 0, scrollBarWidth, position.height); }
    }

    Rect ScrollPositionRect
    {
        get { return new Rect(0, 0, position.width, position.height); }
    }

    Rect AreaRect(bool isThereScrollBar)
    {
        if (isThereScrollBar == true) 
        {
            return new Rect(0, 0, position.width - scrollBarWidth, previousContentRect.height); 
        }
        else
        {
            return new Rect(0, 0, position.width, previousContentRect.height);
        }
    }
    #endregion
}
