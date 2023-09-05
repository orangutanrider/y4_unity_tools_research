using System.Collections.Generic;

public class HelpMessageProviderObj 
{
    public HelpMessageProviderObj(IHelpMessageProvider _provider)
    {
        providerInterface = _provider;

        helpMessages = new List<HelpMessageData>();
    }

    public IHelpMessageProvider providerInterface = null;
    public List<HelpMessageData> helpMessages = null;
}
