
using UnityEngine;

public class EquipmentHolderManager : MonoBehaviour
{

    public ItemHolder[] itemHolders;
    
    
    private void Awake()
    {
 
        itemHolders = GetComponentsInChildren<ItemHolder>();
    }
    
    public void UpdateInventoryDisplay()
    {

        foreach (var holder in itemHolders)
        {
            holder.UpdateSprite();
        }
    }
    
    public void ToggleItemEquipped(int index)
    {

        if (index >= 0 && index < itemHolders.Length)
        {
            itemHolders[index].ToggleEquipped();
            UpdateInventoryDisplay();
        }
        else
        {
            Debug.LogWarning("Índice fuera de rango: " + index);
        }
    }
    
    public void UpdateItemName(int index, string itemName)
    {

        if (index >= 0 && index < itemHolders.Length)
        {
            itemHolders[index].UpdateItemName(itemName);
        }
        else
        {
            Debug.LogWarning("Índice fuera de rango: " + index);
        }
    }
    
    public ItemHolder GetItemHolder(int index)
    {

        if (index >= 0 && index < itemHolders.Length)
        {
            return itemHolders[index];
        }
        else
        {
            Debug.LogWarning("Índice fuera de rango: " + index);
            return null;
        }
    }
    
    public int GetItemHolderCount()
    {

        return itemHolders.Length;
    }
    
    
    
}
