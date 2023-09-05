using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestScript : MonoBehaviour, IHelpMessageProvider
{
    [SerializeField, OpenCustomInspectorButton(typeof(HelpMessageInspector))]
    bool openHelpMessageInspector = false;

    [RequiredReference]
    public GameObject a;

    [NullableRequired]
    public GameObject b;

    [ComponentNullable]
    public GameObject c;

    List<HelpMessageData> IHelpMessageProvider.GetMessages()
    {
        List<HelpMessageData> newList = new List<HelpMessageData>();

        HelpMessageData newMessage = new HelpMessageData("TESTING TEST, TEST, TEST!", MessageType.Warning);

        newList.Add(newMessage);

        return newList;
    }
}
