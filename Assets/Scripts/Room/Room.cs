using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : NetworkBehaviour, IPointerClickHandler
{
    public readonly SyncVar<RoomType> roomType = new SyncVar<RoomType>();
    public NetworkMapSceneManager mapSceneManager;
    public MapUIRoot mapUIRoot;

    public readonly SyncVar<bool> isLock = new SyncVar<bool>();
    public GameObject lockSprite;

    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;

    public TextMeshPro chosenText;
    public readonly SyncVar<int> chosenNum = new SyncVar<int>();//服务器用，几个人选择
    public bool isChosen = false;//客户端用，是否被选中

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLock.Value)
        {
            Debug.Log("房间锁住了");
            return;
        }
        else
        {

        }
        if (isChosen)
        {
            Chosen(false);
        }
        else
        {
            foreach (var room in mapSceneManager.rooms)
            {
                room.Chosen(false);
            }
            Chosen(true);
        }
        //Debug.Log("点击了房间" + roomType.Value.ToString());
        //isLock.Value = true;
        //ClickRpc();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        mapSceneManager = FindObjectOfType<NetworkMapSceneManager>();
        mapSceneManager.rooms.Add(this);
        mapUIRoot = FindObjectOfType<MapUIRoot>();
        transform.SetParent(mapUIRoot.transform);

        isLock.OnChange += IsLock_OnChange;
        chosenNum.OnChange += ChosenNum_OnChange;

        if (roomType.Value == RoomType.SmallEnemy)
        {
            //Debug.Log()
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
        }
    }

    private void ChosenNum_OnChange(int prev, int next, bool asServer)
    {
        if (isLock.Value)
        {
            chosenText.text = "";
            return;
        }
        if (isChosen)
        {
            chosenText.text = $"已选择:{chosenNum.Value}";
        }
        else
        {
            chosenText.text = $"{chosenNum.Value}";
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        isLock.OnChange -= IsLock_OnChange;
        chosenNum.OnChange -= ChosenNum_OnChange;
    }
    [ServerRpc(RequireOwnership = false)]
    public void StartRoomRpc()
    {
        isLock.Value = true;
        if (roomType.Value == RoomType.SmallEnemy)
        {
            mapSceneManager.StartBattle();
        }
        if (roomType.Value == RoomType.Shop)
        {
            mapSceneManager.StartShopRpc();
        }
    }

    public void Chosen(bool b)
    {
        if (isChosen == b)
        {
            return;
        }
        else
        {

        }
        isChosen = b;
        Debug.Log("投票" + b);
        ChosenRpc(isChosen);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChosenRpc(bool b)
    {
        if (b)
        {
            chosenNum.Value++;

        }
        else
        {
            chosenNum.Value--;
        }
        Debug.Log("有人投票了,这个房间的投票数为" + chosenNum.Value);
        Debug.Log("总人数" + InstanceFinder.NetworkManager.ClientManager.Clients.Count);
        if (chosenNum.Value == InstanceFinder.NetworkManager.ClientManager.Clients.Count)
        {
            StartRoomRpc();
        }
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
