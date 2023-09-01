using System.Collections.Generic;
using UnityEditor;

public class HelpMessageProviderObj 
{
    public HelpMessageProviderObj(IHelpMessageProvider _provider, SerializedObject _serializedObject)
    {
        provider = _provider;
        serializedObject = _serializedObject;

        helpMessages = new List<HelpMessageData>();
    }


    public IHelpMessageProvider provider = null;
    public SerializedObject serializedObject = null;

    public List<HelpMessageData> helpMessages = null;
}
