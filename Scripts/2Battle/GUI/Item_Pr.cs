using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "New Item/item")]
public class Item_Pr : ScriptableObject
{
    public string itmeName;
    public ItemType itemType;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;
    // Start is called before the first frame update
    public enum ItemType
    {
        Used,
        Ingredient,
        ETC
    }


}
