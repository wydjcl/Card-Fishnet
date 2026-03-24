using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRpcManager : NetworkBehaviour
{
    public static NetworkRpcManager instance;
    public NetworkUI networkUI;
    public void Awake()
    {
        instance = this;
    }
    [ObserversRpc]
    public void ExitLobby()
    {
        networkUI.lobbyNode.SetActive(false);
    }
}
