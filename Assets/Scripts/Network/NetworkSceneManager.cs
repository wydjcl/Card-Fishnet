using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneManager : MonoBehaviour
{
    /// <summary>
    /// 加载地图场景
    /// </summary>
    [ContextMenu("加载地图场景")]
    public void LoadMapScene()
    {
        if (!InstanceFinder.NetworkManager.IsServerStarted)
        {
            return;
        }
        SceneLoadData sld = new SceneLoadData("MapScene");
        InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        NetworkRpcManager.instance.ExitLobby();
    }
}
