using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    
    // left and right hand items
    public InventoryItem leftHandItem;
    public InventoryItem rightHandItem;
    
    // current clothing wearing
    public ClothingSO currentClothingItem;

    private List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Inventory created and set to DontDestroyOnLoad.");
        }
        else
        {
            Debug.Log("Duplicate Inventory found. Destroying.");
            Destroy(gameObject);
        }
    }

    public void AddItemSO(ItemSO itemToAdd, int amount = 1)
    {
        var invItem = items.Find(i => i.itemData == itemToAdd);
        if (invItem != null)
            invItem.quantity += amount;
        else
            items.Add(new InventoryItem(itemToAdd, amount));

        Debug.Log($"Added {amount}x {itemToAdd.itemName} to inventory");
    }

    public bool HasKey(string keyID)
    {
        foreach(var invItem in items)
        {
            if (invItem.itemData is KeySO key && key.keyID == keyID && invItem.quantity > 0)
                return true;
        }
        return false;
    }

    public bool RemoveItem(ItemSO itemToRemove, int amount = 1)
    {
        var invItem = items.Find(i => i.itemData == itemToRemove);
        if (invItem != null && invItem.quantity >= amount)
        {
            invItem.quantity -= amount;
            if (invItem.quantity <= 0) items.Remove(invItem);
            Debug.Log($"Removed {amount}x {itemToRemove.itemName} from inventory");
            return true;
        }
        return false;
    }

    public List<InventoryItem> GetAllItems()
    {
        Debug.Log("Inventory contents:");
        foreach (var item in items)
        {
            Debug.Log($" - {item.itemData.itemName} x{item.quantity}");
        }
        return new List<InventoryItem>(items);
    }
    
    
    public List<InventoryItem> GetKeyItems()
    {
        return items.FindAll(i => i.itemData is KeySO);
    }
    
    
    public void AddItems(List<InventoryItem> itemsToAdd)
    {
        foreach (var item in itemsToAdd)
        {
            AddItemSO(item.itemData, item.quantity);
        }
    }
    
}