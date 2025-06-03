
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder: MonoBehaviour
{
    // Guardar dos imagenes de un objeto para ui y hacer un toggle para cuando el item esté equipado o no y que se vea en un image de canvas
    public Sprite itemSprite;
    public Sprite itemSpriteEquipped;
    public bool isEquipped = false;
    
    // conseguir textmesh pro de un item y actualizarlo
    public TMPro.TextMeshProUGUI itemNameText;
    
    // actualizar el texto del item
    public void UpdateItemName(String itemName)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }
    }
    
    public void ToggleEquipped()
    {
        isEquipped = !isEquipped;
        UpdateSprite();
    }
    
    public void UpdateSprite()
    {
        if (isEquipped)
        {
            GetComponent<Image>().sprite = itemSpriteEquipped;
        }
        else
        {
            GetComponent<Image>().sprite = itemSprite;
        }
    }
    
    private void Start()
    {
        UpdateSprite();
    }
    
    
}
