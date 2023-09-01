using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestScript : MonoBehaviour, IHelpMessageProvider
{
    [OpenCustomInspectorButton(typeof(HelpMessageInspector))]

    public float e;

    List<HelpMessageData> IHelpMessageProvider.GetMessages()
    {
        List<HelpMessageData> newList = new List<HelpMessageData>();

        HelpMessageData newMessage = new HelpMessageData("TESTING TEST, TEST, TEST!", MessageType.Warning);

        newList.Add(newMessage);

        return newList;
    }
}
