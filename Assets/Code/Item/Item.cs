[System.Serializable]
public class Item
{
    public string name;
    public string description;
    public ItemCategory category;

    public Item(string name, string description, ItemCategory category)
    {
        this.name = name;
        this.description = description;
        this.category = category;
    }
}