using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CommentCompiler))]
public class CommentCompilerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CommentCompiler commentCompiler = (CommentCompiler)target;


        if (GUILayout.Button("TestButton"))
        {
            commentCompiler.TestButton();
        }
    }
}
