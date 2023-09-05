using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class HelpMessagesComponentObj 
{
    public HelpMessagesComponentObj(SerializedObject _serializedObject)
    {
        serializedObject = _serializedObject;

        editorsMessages = new List<HelpMessageEditorProviderObj>();
    }

    public SerializedObject serializedObject = null;

    public HelpMessageProviderObj componentMessages = null;
    public List<HelpMessageEditorProviderObj> editorsMessages = null;
}
