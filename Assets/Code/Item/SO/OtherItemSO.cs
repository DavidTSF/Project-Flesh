using UnityEngine;

[CreateAssetMenu(fileName = "NewOtherItem", menuName = "Items/Other")]
public class OtherItemSO : ItemSO
{
    public OtherItemSO(string itemName, string description, ItemCategory category, Sprite icon) : base(itemName, description, category, icon)
    {
    }
}