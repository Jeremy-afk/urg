using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    private void Awake()
    {
        //DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) { return; }

        /*string portString = GetArg("-port");

        KcpTransport transport = MyNetworkRoomManager.singleton.GetComponent<KcpTransport>();
        ushort port;
        if (ushort.TryParse(portString, out port))
        {
            Debug.Log($"Conversion réussie : {port}");
            transport.port = port;
        }
        else
        {
            Debug.LogError("Échec de la conversion : la chaîne n'est pas un nombre valide ou est hors plage.");
        }*/

    }
    // Helper function for getting the command line arguments
    private string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            //Debug.Log(args[i]);
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return "7777";
    }
}
