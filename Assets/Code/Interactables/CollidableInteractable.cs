using UnityEngine;

public abstract class CollidableInteractable : Interactable
{
    protected Collider2D interactionCollider;
    protected Collider2D collisionCollider;
    
    protected virtual void Awake()
    {
        SetInteractType(INTERACT_TYPE.COLLIDER);
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
    }

    protected void SetCollisionState(bool canPass)
    {
        if (collisionCollider != null)
            collisionCollider.enabled = !canPass;
    }
}
