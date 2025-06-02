using UnityEngine;

[CreateAssetMenu(fileName = "NewClothing", menuName = "Items/Clothing")]
public class ClothingSO : ItemSO
{
    public int defense;
    public float durability;
    public Sprite sprite;

    public ClothingSO(string itemName, string description, ItemCategory category, Sprite icon, int defense, float durability) : base(itemName, description, category, icon)
    {
        this.defense = defense;
        this.durability = durability;
    }

    public ClothingSO(string itemName, string description, ItemCategory category, Sprite icon, int defense, float durability, Sprite sprite) : base(itemName, description, category, icon)
    {
        this.defense = defense;
        this.durability = durability;
        this.sprite = sprite;
    }
}