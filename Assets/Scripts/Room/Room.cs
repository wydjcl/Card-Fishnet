using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : NetworkBehaviour, IPointerClickHandler
{
    public RoomType roomType;
    public NetworkMapSceneManager mapSceneManager;
    public MapUIRoot mapUIRoot;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击了房间" + roomType.ToString());
        ClickRpc();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        mapSceneManager = FindObjectOfType<NetworkMapSceneManager>();
        mapUIRoot = FindObjectOfType<MapUIRoot>();
        transform.SetParent(mapUIRoot.transform);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClickRpc()
    {
        mapSceneManager.StartBattle();
    }
}
