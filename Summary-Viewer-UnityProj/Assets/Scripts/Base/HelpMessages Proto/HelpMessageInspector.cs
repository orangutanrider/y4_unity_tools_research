using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

using EditorUtilities.AttributedSerializedExtensions;

public class HelpMessageInspector : CustomInspector
{
    List<HelpMessagesComponentObj> inspectedComponents = new List<HelpMessagesComponentObj>();

    private void OnGUI()
    {
        StartGUIContent();

        foreach (HelpMessagesComponentObj component in inspectedComponents)
        {
            bool areThereObjectMessages = DoesComponentContainAnyObjectMessages(component);
            bool areThereEditorMessages = DoesComponentContainAnyEditorMessage(component);

            if (areThereObjectMessages == false && areThereEditorMessages == false) { continue; }

            EditorGUILayout.InspectorTitlebar(true, component.serializedObject.targetObject);

            if (areThereObjectMessages == true)
            {
                DrawObjectMessages(component.componentMessages);
            }

            if (areThereObjectMessages == true && areThereEditorMessages == true)
            {
                DrawInspectorEndLine(); // replace this with something different in the refactor, make the line not cut across the entire window
            }

            if (areThereEditorMessages == true)
            {
                foreach (HelpMessageEditorProviderObj editorObj in component.editorsMessages)
                {
                    DrawEditorMessages(editorObj);
                }
            }
        }

        DrawInspectorEndLine();

        EndGUIContent();
    }

    private void OnSelectionChange()
    {
        repaintBuffer = 0;

        if (Locked == true)
        {
            GetEditorMessages();
            Repaint();
            return;
        }

        if (TryGetNewInspectedViaGameObj(Selection.activeGameObject) == true)
        {
            GetObjectMessages();
            GetEditorMessages();
            Repaint();
            return;
        }

        if (TryGetNewInspectedViaObj(Selection.activeObject) == true)
        {
            GetObjectMessages();
            GetEditorMessages();
            Repaint();
            return;
        }

        Debug.LogWarning("E");
    }

    protected override void ButtonUnlocked()
    {
        OnSelectionChange();
    }

    public static void RequestMessageUpdate()
    {
        if (HasOpenInstances<HelpMessageInspector>() == false) { return; }

        HelpMessageInspector window = GetWindow<HelpMessageInspector>();
        window.GetEditorMessages();
        window.repaintBuffer = 0;
        window.Repaint();
    }

    bool TryGetNewInspectedViaGameObj(GameObject gameObject)
    {
        if (gameObject == null) { return false; }

        MonoBehaviour[] newInspected = gameObject.GetComponents<MonoBehaviour>();

        inspectedComponents.Clear();
        foreach (MonoBehaviour inspected in newInspected)
        {
            HelpMessagesComponentObj helpMsgObj = new HelpMessagesComponentObj(new SerializedObject(inspected));
            inspectedComponents.Add(helpMsgObj);
        }
        return true;
    }

    bool TryGetNewInspectedViaObj(Object obj)
    {
        if (obj == null) { return false; }

        inspectedComponents.Clear();
        HelpMessagesComponentObj helpMsgObj = new HelpMessagesComponentObj(new SerializedObject(obj));
        inspectedComponents.Add(helpMsgObj);
        return true;
    }

    #region Editor Messages
    void GetEditorMessages()
    {
        HelpMessageTypeStorage typeStorage = Resources.Load<HelpMessageTypeStorage>("HelpMessageTypeStorage");
        if (typeStorage == null) { return; }
        if (typeStorage.methodList == null) { return; }
        if (typeStorage.methodList.Count == 0) { return; }

        foreach (HelpMessagesComponentObj component in inspectedComponents)
        {
            component.editorsMessages.Clear();
        }

        foreach (MethodInfo method in typeStorage.methodList)
        {
            #region Check if the method is valid
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

            if (methodParams.Length != 1)
            {
                Debug.LogError("C");
                continue;
            }

            if (methodParams[0].ParameterType != typeof(SerializedObject))
            {
                Debug.LogError("D");
                continue;
            }
            #endregion

            foreach (HelpMessagesComponentObj component in inspectedComponents)
            {
                if (component == null) { continue; }
                if (component.serializedObject == null) { continue; }

                object[] parameters = { component.serializedObject };

                List<HelpMessageData> editorMessages = (List<HelpMessageData>)method.Invoke(this, parameters);
                HelpMessageEditorProviderObj loopReturn = new HelpMessageEditorProviderObj(method.DeclaringType);
                loopReturn.helpMessages = editorMessages; // in the refactor make it optional to include this in the decleration

                component.editorsMessages.Add(loopReturn);
            }
        }
    }

    void DrawEditorMessages(HelpMessageEditorProviderObj helpMessageObj)
    {
        foreach (HelpMessageData messageData in helpMessageObj.helpMessages)
        {
            if (messageData.active == false) { continue; }
            EditorGUILayout.HelpBox("(" + helpMessageObj.editorType.Name + ") " + messageData.message, messageData.messageType, true);
        }
    }

    bool DoesComponentContainAnyEditorMessage(HelpMessagesComponentObj component)
    {
        if (component == null) { return false; }
        if (component.editorsMessages == null) { return false; }

        foreach (HelpMessageEditorProviderObj editorMessages in component.editorsMessages)
        {
            if (editorMessages == null) { continue; }
            if (editorMessages.helpMessages == null) { continue; }

            foreach (HelpMessageData messageData in editorMessages.helpMessages)
            {
                if (messageData == null) { continue; }
                if (messageData.active == true) { return true; }
            }
        }

        return false;
    }
    #endregion

    #region Object Messages
    void GetObjectMessages()
    {
        foreach (HelpMessagesComponentObj component in inspectedComponents)
        {
            System.Type[] interfaces = component.serializedObject.targetObject.GetType().GetInterfaces();
            if (interfaces == null) { continue; }

            if (ValidInterfaceExists(interfaces) == false) { continue; }

            IHelpMessageProvider msgProviderInterface = (IHelpMessageProvider)component.serializedObject.targetObject;
            HelpMessageProviderObj msgProvider = new HelpMessageProviderObj(msgProviderInterface);
            msgProvider.helpMessages = msgProviderInterface.GetMessages();

            component.componentMessages = msgProvider;
        }
    }

    bool ValidInterfaceExists(System.Type[] interfaces)
    {
        foreach (System.Type providerInterface in interfaces)
        {
            if (providerInterface == typeof(IHelpMessageProvider))
            {
                return true;
            }
        }
        return false;
    }

    void DrawObjectMessages(HelpMessageProviderObj helpMessageObj)
    {
        foreach (HelpMessageData messageData in helpMessageObj.helpMessages)
        {
            if (messageData.active == false) { continue; }
            EditorGUILayout.HelpBox(messageData.message, messageData.messageType, true);
        }
    }

    bool DoesComponentContainAnyObjectMessages(HelpMessagesComponentObj component)
    {
        if (component == null) { return false; }
        if (component.componentMessages == null) { return false; }

        if (component.componentMessages.helpMessages == null) { return false; }

        foreach (HelpMessageData messageData in component.componentMessages.helpMessages)
        {
            if (messageData == null) { continue; }
            if (messageData.active == true) { return true; }
        }

        return false;
    }
    #endregion
}
