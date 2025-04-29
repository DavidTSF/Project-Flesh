using UnityEngine;

public class ItemKey : Interactable
{
    public string keyID = ""; // ID de la llave que se recogerá
    
    private void Start()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Este objeto necesita un Collider2D!");
        }
    }

    public override bool Interact()
    {
        // Añadir la llave al inventario
        Inventory.Instance.AddKey(keyID);
        Destroy(gameObject);
        Debug.Log("Has recogido la llave: " + keyID);
        return true; 
    }
}