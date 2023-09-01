using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorUtilities.AttributedSerializedExtensions;

public class HelpMessageInspector : CustomInspector
{
    List<HelpMessageProviderObj> inspectedProviders = new List<HelpMessageProviderObj>();

    private void OnGUI()
    {
        StartGUIContent();

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

        HelpMessageTypeStorage typeStorage = Resources.Load<HelpMessageTypeStorage>("HelpMessageTypeStorage");
        Debug.Log(typeStorage);

        if (typeStorage == null) { return; }
        if (typeStorage.methodList == null) { return; }
        if (typeStorage.methodList.Count == 0) { return; }

        Debug.Log(typeStorage.methodList[0].Name);
        typeStorage.methodList[0].Invoke(this, null);
    }
}
