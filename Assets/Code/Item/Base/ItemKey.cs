
public class ItemKey : Item
{
    public string keyID;

    public ItemKey(string name, string description, ItemCategory category, string keyID) : base(name, description, category)
    {
        this.keyID = keyID;
    }
}

