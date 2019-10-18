using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public Item_Pr item;
    public int itemCount;
    public Image itemIamge;
    public GameObject itemButton;

    //必要なconponent
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CpimtImage;

    //imageの透明
    private void SetColor(float _alpha)
    {
        Color color = itemIamge.color;
        color.a = _alpha;
        itemIamge.color = color;
    }

    //Item獲得
    public void AddItem(Item_Pr _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemIamge.sprite = item.itemImage;
        itemButton.SetActive(true);




        go_CpimtImage.SetActive(true);
        text_Count.text = itemCount.ToString();

        SetColor(1);

    }

    //item回数調査
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    //slot初期化
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemIamge.sprite = null;
        itemButton.SetActive(false);
        SetColor(0);

        
        text_Count.text = "0";
        go_CpimtImage.SetActive(false);
    }

    public void UseItemButton()
    {
        var BSM = FindObjectOfType<BattleStateMachine>();
        BSM.Input7();
        SetSlotCount(-1);
    }


}
