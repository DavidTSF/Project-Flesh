using UnityEngine;

public class LockedDoor : Door
{
    public string requiredKeyID;
    
    public override bool Interact()
    {
        if (isOpen)
        {
            base.Interact(); 
            return true;
        }

        // Si está cerrada, verificamos si el jugador tiene la llave
        if (Inventory.Instance.HasKey(requiredKeyID))
        {
            Debug.Log("Puerta desbloqueada con la llave.");
            base.Interact();
            return true;
        }

        Debug.Log("La puerta está cerrada. Necesitas una llave.");
        return false;
    }


    
}
