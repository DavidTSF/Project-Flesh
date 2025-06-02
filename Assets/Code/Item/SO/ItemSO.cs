using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public ItemCategory category;
    public Sprite icon;
    public bool isInUse = false;

    protected ItemSO(string itemName, string description, ItemCategory category, Sprite icon)
    {
        this.itemName = itemName;
        this.description = description;
        this.category = category;
        this.icon = icon;
    }
}