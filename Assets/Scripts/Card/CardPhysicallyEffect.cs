using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class CardPhysicallyEffect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Card card;

    public bool isDrag;

    [HideInInspector]
    public bool canEcecute;

    public GameObject arrowPrefab;
    private GameObject currentArrow;
    public Character target;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (card.player.MP < card.cardCost)
        //{
        //    return;
        //}
        if (card.isAni)
        {
            return;
        }
        card.Entry.transform.position = transform.position;//
        if (card.cardType == CardType.AttackCard || card.cardType == CardType.PAffectCard)
        {
            currentArrow = Instantiate(arrowPrefab, transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (card.player.MP < card.cardCost)
        //{
        //    return;
        //}
        if (card.isAni)
        {
            return;
        }
        isDrag = true;
        card.Entry.transform.position = transform.position;
        if (card.cardType != CardType.AttackCard && card.cardType != CardType.PAffectCard)
        {
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);// 屏幕坐标0-1920
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            card.transform.position = new Vector3(worldPos.x, worldPos.y, -1);
            card.GetComponent<SortingGroup>().sortingOrder = 50;
            canEcecute = worldPos.y > -1.8f;//拉到1f上就用用掉
                                            //  target = card.battleManager.GetMainPlayer();
        }
        else
        {
            if (eventData.pointerEnter != null)
            {
                if (eventData.pointerEnter.CompareTag("Enemy") && card.cardType == CardType.AttackCard)
                {
                    if (eventData.pointerEnter.GetComponent<Enemy>() != null)
                    {
                        target = eventData.pointerEnter.GetComponent<Enemy>();
                        canEcecute = true;
                    }
                }
                if (eventData.pointerEnter.CompareTag("Player") && card.cardType == CardType.PAffectCard)
                {
                    if (eventData.pointerEnter.GetComponent<Player>() != null)
                    {
                        target = eventData.pointerEnter.GetComponent<Player>();
                        canEcecute = true;
                    }
                }
            }
            else
            {
                target = null;
                canEcecute = false;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        transform.position = card.originalPosition;
        card.Entry.transform.position = transform.position;
        //card.GetComponent<SortingGroup>().sortingOrder = card.originaLayerOrder;
        if (currentArrow != null)
        {
            Destroy(currentArrow.gameObject);
        }
        if (canEcecute)
        {
            //Debug.Log("触发卡牌");
            //if (target != null)
            //{
            //    Debug.Log("target:" + target.name);
            //}
            //card.Effect(PlayerPointer.Instance.player.playerInMap, target);
            //// card.player.MP -= card.cardCost;
            ////card.player.UpdateUI();
            if (target != null)
            {
                //Debug.Log("target为" + target.name);
            }
            card.UseCard(card.player.myPlayer, target);
            target = null;
        }
        canEcecute = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card.isAni)//防止抽卡途中扰乱Entry坐标
        {
            return;
        }
        var s = card.GetComponent<SortingGroup>();
        s.sortingOrder = 999;
        if (!isDrag)
        {
            //card.Entry.transform.position = card.originalPosition + Vector3.up * 1.95f;
            card.Entry.transform.position = card.transform.position + Vector3.up * 1.95f;
        }
        //  Debug.Log()
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (card.isAni)//防止抽卡途中扰乱Entry坐标
        {
            return;
        }
        var s = card.GetComponent<SortingGroup>();
        s.sortingOrder = card.orSortingOrder;
        //card.Entry.transform.position = card.originalPosition;
        card.Entry.transform.position = card.transform.position;
    }
}