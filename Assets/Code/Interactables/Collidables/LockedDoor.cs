using UnityEngine;


public class LockedDoor : Door
{
    [SerializeField] private KeySO requiredKeyID;
    
    protected override void Awake()
    {
        base.Awake();
        if (requiredKeyID == null)
        {
            Debug.LogError("🔒 La llave requerida no está asignada en la puerta bloqueada.");
        }
    }
    
    public override InteractionResult Interact(IInteractor interactor)
    {
        if (isOpen)
        {
            return base.Interact(interactor);
        }
        if (Inventory.Instance.HasKey(requiredKeyID.keyID))
        {
            //Debug.Log("Puerta desbloqueada con la llave.");
            return base.Interact(interactor);
        }
        
        return InteractionResult.Failed;
    }

    public override string GetInteractTip()
    {
        return isOpen ? "Cerrar puerta" : "Abrir (necesita llave)";
    }

    public override bool CanInteract()
    {
        return isOpen || Inventory.Instance.HasKey(requiredKeyID.keyID);
    }
}