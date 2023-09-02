using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

using EditorUtilities.AttributedSerializedExtensions;

public class HelpMessageInspector : CustomInspector
{
    List<HelpMessageProviderObj> inspectedProviders = new List<HelpMessageProviderObj>();

    List<HelpMessageEditorProviderObj> editorMessages = new List<HelpMessageEditorProviderObj>();

    private void OnGUI()
    {
        StartGUIContent();

        // Should probably add formatting options for the editors
        // As in somekind of API thing
        // This'd be in the interest of allowing them to post their messages under inspector title bars of select components
        // Though, maybe that isn't needed
        // No yeh, I should definetly do that, otherwise these messages don't really make any sense
        if (editorMessages != null)
        {
            foreach (HelpMessageEditorProviderObj editorMessage in editorMessages)
            {
                EditorGUILayout.LabelField(editorMessage.type.Name);
                foreach (HelpMessageData messageData in editorMessage.helpMessages)
                {
                    if (messageData.active == true)
                    {
                        EditorGUILayout.HelpBox(messageData.message, messageData.messageType, true);
                    }
                }
            }
        }

        foreach(HelpMessageProviderObj provider in inspectedProviders)
        {
            EditorGUILayout.InspectorTitlebar(true, provider.serializedObject.targetObject);

            foreach(HelpMessageData helpMessageData in provider.helpMessages)
            {
                if (helpMessageData.active == true)
                {
                    EditorGUILayout.HelpBox(helpMessageData.message, helpMessageData.messageType, true);
                }
            }
        }

        DrawInspectorEndLine();

        EndGUIContent();
    }

    protected override void SelectionUpdate(Object newSelectedObject, GameObject newSelectedGameObject)
    {
        IHelpMessageProvider[] newProviders = newSelectedGameObject.GetComponents<IHelpMessageProvider>();

        inspectedProviders.Clear();
        foreach(IHelpMessageProvider provider in newProviders)
        {
            HelpMessageProviderObj newProvider = new HelpMessageProviderObj(provider, new SerializedObject((MonoBehaviour)provider));
            inspectedProviders.Add(newProvider);
            newProvider.helpMessages = provider.GetMessages();
        }
    }

    private void OnSelectionChange()
    {
        //HelpMessageTypeStorage typeStorage = Resources.Load<HelpMessageTypeStorage>("Assets/Scripts/proto/HelpMessageTypeStorage.asset");
        //Debug.Log(typeStorage);
        // Returns null

        //HelpMessageTypeStorage typeStorage = Resources.GetBuiltinResource<HelpMessageTypeStorage>("Assets/Scripts/proto/HelpMessageTypeStorage.asset");
        //Debug.Log(typeStorage);

        /*
        Debug.Log(typeStorage.methodList[0].Name);
        object[] parameters = { Selection.activeObject, Selection.activeGameObject };
        List<HelpMessageData> messageData = (List<HelpMessageData>)typeStorage.methodList[0].Invoke(this, parameters);
        */

        editorMessages = GetEditorMessages();
    }

    List<HelpMessageEditorProviderObj> GetEditorMessages()
    {
        HelpMessageTypeStorage typeStorage = Resources.Load<HelpMessageTypeStorage>("HelpMessageTypeStorage");

        if (typeStorage == null) { return null; }
        if (typeStorage.methodList == null) { return null; }
        if (typeStorage.methodList.Count == 0) { return null; }

        // make this a readonly variable in the refactor
        object[] parameters = { Selection.activeObject, Selection.activeGameObject };

        List<HelpMessageEditorProviderObj> returnList = new List<HelpMessageEditorProviderObj>();

        foreach (MethodInfo method in typeStorage.methodList)
        {
            if (method.ReturnType != typeof(List<HelpMessageData>))
            {
                Debug.LogError("A");
                continue;
            }

            ParameterInfo[] methodParams = method.GetParameters();
            if (methodParams == null)
            {
                Debug.LogError("B");
                continue;
            }

            if (methodParams.Length != 2)
            {
                Debug.LogError("C");
                continue;
            }

            if(methodParams[0].ParameterType != typeof(Object))
            {
                Debug.LogError("D");
                continue;
            }

            if (methodParams[1].ParameterType != typeof(GameObject))
            {
                Debug.LogError("E");
                continue;
            }

            List<HelpMessageData> editorMessages = (List<HelpMessageData>) method.Invoke(this, parameters);
            HelpMessageEditorProviderObj loopReturn = new HelpMessageEditorProviderObj(method.DeclaringType);
            loopReturn.helpMessages = editorMessages; // in the refactor make it optional to include this in the decleration

            returnList.Add(loopReturn);
        }

        return returnList;
    }
}
