using UnityEngine;

[CreateAssetMenu(fileName = "NewKey", menuName = "Items/Key")]
public class KeySO : ItemSO
{
    public string keyID;

    public KeySO(string itemName, string description, ItemCategory category, Sprite icon, string keyID) : base(itemName, description, category, icon)
    {
        this.keyID = keyID;
    }
}