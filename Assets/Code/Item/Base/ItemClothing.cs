
public class ItemClothing : Item 
{
    public int defense;
    public float durability;

    public ItemClothing(string name, string description, ItemCategory category, int defense, float durability)
        : base(name, description, category)
    {
        this.defense = defense;
        this.durability = durability;
    }
}