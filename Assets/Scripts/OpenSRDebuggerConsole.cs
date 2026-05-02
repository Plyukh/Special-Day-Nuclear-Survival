using UnityEngine;
using SRDebugger;

public class OpenSRDebuggerConsole : MonoBehaviour
{
    public void OpenConsole()
    {
        if (!SRDebug.IsInitialized)
            SRDebug.Init();

        SRDebug.Instance.ShowDebugPanel(DefaultTabs.Console, false);
    }
}
