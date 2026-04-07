using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public ShopCardUI shopCardUI0;
    public ShopCardUI shopCardUI1;
    public ShopCardUI shopCardUI2;
    public ShopCardUI shopCardUI3;
    public ShopCardUI shopCardUI4;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        var r = Dic.Instance.GetCardsRandom(Dic.Instance.player.characterId.Value, 5);
        shopCardUI0.data = r[0];
        shopCardUI0.Init();
        shopCardUI0.gameObject.SetActive(true);

        shopCardUI1.data = r[1];
        shopCardUI1.Init();
        shopCardUI1.gameObject.SetActive(true);

        shopCardUI2.data = r[2];
        shopCardUI2.Init();
        shopCardUI2.gameObject.SetActive(true);

        shopCardUI3.data = r[3];
        shopCardUI3.Init();
        shopCardUI3.gameObject.SetActive(true);

        shopCardUI4.data = r[4];
        shopCardUI4.Init();
        shopCardUI4.gameObject.SetActive(true);
    }

    public void Quit()
    {
        gameObject.SetActive(false);
    }
}
