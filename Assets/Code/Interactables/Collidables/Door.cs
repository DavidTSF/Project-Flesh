using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected Sprite openSprite;
    [SerializeField] protected Sprite closedSprite;
    
    protected Collider2D interactionCollider;
    protected Collider2D collisionCollider;
    
    private SpriteRenderer spriteRenderer;
    protected bool isOpen = false;
    protected bool used = false;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
                interactionCollider = col;
            else
                collisionCollider = col;
        }

        if (interactionCollider == null || collisionCollider == null)
        {
            Debug.LogWarning($"[{name}] No se encontraron ambos colliders correctamente.");
        }
        UpdateVisual();
    }

    public virtual InteractionResult Interact(IInteractor interactor)
    {
        if (!CanInteract()) return InteractionResult.Failed;

        isOpen = !isOpen;
        UpdateVisual();
        used = true;
        return InteractionResult.Success;
    }

    public virtual  string GetInteractTip() => isOpen ? "Cerrar puerta" : "Abrir puerta";
    public virtual bool CanInteract() => true;

    public virtual bool IsOneTimeUse() => false;

    public virtual bool IsActive() => true;

    private void UpdateVisual()
    {
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
        gameObject.layer = isOpen ? 0 : 3;
        if (collisionCollider != null) collisionCollider.enabled = !isOpen;
    }
}