using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NetworkLobbyNode : MonoBehaviour
{
    private NetworkPlayer player;
    public Image characterImage;
    public List<Sprite> characterImages = new List<Sprite>();
    public void Click0()
    {
        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        player.ChangeCharacterIdTo0();
        characterImage.sprite = characterImages[0];
    }
    public void Click1()
    {
        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        player.ChangeCharacterIdTo1();
        characterImage.sprite = characterImages[1];
    }
}
