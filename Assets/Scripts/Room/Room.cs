using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : NetworkBehaviour, IPointerClickHandler
{
    public readonly SyncVar<RoomType> roomType = new SyncVar<RoomType>();
    public NetworkMapSceneManager mapSceneManager;
    public MapUIRoot mapUIRoot;
    public List<Sprite> sprites;
    public readonly SyncVar<bool> isLock = new SyncVar<bool>();
    public GameObject lockSprite;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsServerStarted)
        {
            return;
        }
        if (isLock.Value)
        {
            Debug.Log("房间锁住了");
            //return;
        }
        else
        {

        }
        Debug.Log("点击了房间" + roomType.ToString());
        isLock.Value = true;
        ClickRpc();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        mapSceneManager = FindObjectOfType<NetworkMapSceneManager>();
        mapSceneManager.rooms.Add(this);
        mapUIRoot = FindObjectOfType<MapUIRoot>();
        transform.SetParent(mapUIRoot.transform);

        isLock.OnChange += IsLock_OnChange;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        isLock.OnChange -= IsLock_OnChange;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClickRpc()
    {
        mapSceneManager.StartBattle();
    }

    private void IsLock_OnChange(bool prev, bool next, bool asServer)
    {
        lockSprite.SetActive(isLock.Value);
    }
    [ContextMenu("输出房间类型")]
    public void D()
    {
        Debug.Log(roomType.Value.ToString());
    }
}
