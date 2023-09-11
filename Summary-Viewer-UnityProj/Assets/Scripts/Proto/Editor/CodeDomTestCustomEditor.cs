using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CodeDomTest))]
public class CodeDomTestCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CodeDomTest codeDomTest = (CodeDomTest)target;

        if (GUILayout.Button("Execute: Microsoft Readme Test"))
        {
            codeDomTest.MicrosoftReadMeExample();
        }
    }
}
