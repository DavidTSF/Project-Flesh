using UnityEngine;

public class ObjectItem : MonoBehaviour, IInteractable
{
    public ItemSO itemData;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogWarning("No SpriteRenderer found on ObjectItem.");
    }

    private void Start()
    {
        if (itemData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
    }

    public InteractionResult Interact(IInteractor interactor)
    {
        Inventory.Instance.AddItemSO(itemData);
        Destroy(gameObject);
        Debug.Log($"Picked up {itemData.itemName}");
        return InteractionResult.Success;
    }

    public string GetInteractTip()
    {
        return $"Pick up {itemData.itemName}";
    }

    public bool CanInteract() => true;

    public bool IsOneTimeUse() => true;

    public bool IsActive() => true;
}