using UnityEngine;

public class DoorOld : CollidableInteractable
{
    public Sprite openSprite;
    public Sprite closedSprite;

    protected SpriteRenderer spriteRenderer;
    protected bool isOpen = false;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public override bool Interact()
    {
        isOpen = !isOpen;
        UpdateVisual();
        return true; 
    }

    protected virtual void UpdateVisual()
    {
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
        gameObject.layer = isOpen ? 0 : 3;
        SetCollisionState(isOpen);
    }
}