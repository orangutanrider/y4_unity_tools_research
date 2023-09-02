using System.Collections.Generic;
using System;

public class HelpMessageEditorProviderObj 
{
    public HelpMessageEditorProviderObj(Type _type)
    {
        type = _type;
        helpMessages = new List<HelpMessageData>();
    }

    public Type type = null;
    public List<HelpMessageData> helpMessages = null;
}
