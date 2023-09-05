using System.Collections.Generic;
using System;

public class HelpMessageEditorProviderObj 
{
    public HelpMessageEditorProviderObj(Type _editorType)
    {
        editorType = _editorType;
        helpMessages = new List<HelpMessageData>();
    }

    public Type editorType = null;
    public List<HelpMessageData> helpMessages = null; 
}
