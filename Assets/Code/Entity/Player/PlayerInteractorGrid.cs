using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractorGrid : MonoBehaviour
{
    public int interactRadius = 1;
    public GameObject selectorPrefab;
    public GameObject floorPrefab;
    
    private GridMovementV2 gridMovement;
    private Vector2Int currentSelectionOffset = Vector2Int.zero;
    private GameObject selectorInstance;
    private GameObject floorInstance;
    public bool isSelecting = false;
    private List<Vector2Int> validOffsets;
    private PlayerControllerV2 playerController;
    
    
    void Start()
    {
        gridMovement = GetComponent<GridMovementV2>();
        validOffsets = GenerateOffsets(interactRadius);
    }

    public void ToggleInteractionMode()
    {
        isSelecting = !isSelecting;

        if (isSelecting)
        {
            currentSelectionOffset = Vector2Int.zero;
            ShowSelector();
        }
        else
        {
            HideSelector();
        }
    }

    public void MoveSelector(Vector2Int direction)
    {
        if (!isSelecting) return;

        Vector2Int newOffset = currentSelectionOffset + direction;
        if (validOffsets.Contains(newOffset))
        {
            currentSelectionOffset = newOffset;
            UpdateSelectorPosition();
        }
    }

    public void ConfirmInteraction()
    {
        if (!isSelecting) return;

        Vector2Int gridOrigin = gridMovement.currentGridPos;
        Vector2Int targetPos = gridOrigin + currentSelectionOffset;

        Vector3 worldPos = gridMovement.GridToWorld(targetPos);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                InteractionResult result = interactable.Interact(playerController); 
                break;
            }
        }
    }

    void ShowSelector()
    {
        if (selectorPrefab == null) return;
        if (floorPrefab == null) return;

        if (selectorInstance == null)
            selectorInstance = Instantiate(selectorPrefab);
        
        if (floorInstance == null)
            floorInstance = Instantiate(floorPrefab);
        
        floorInstance.transform.position = gridMovement.GridToWorld(gridMovement.currentGridPos);
        UpdateSelectorPosition();
        selectorInstance.SetActive(true);
        floorInstance.SetActive(true);
    }

    void UpdateSelectorPosition()
    {
        Vector3 worldPos = gridMovement.GridToWorld(gridMovement.currentGridPos + currentSelectionOffset);
        selectorInstance.transform.position = worldPos;
        
    }

    void HideSelector()
    {
        if (selectorInstance != null)
            selectorInstance.SetActive(false);
        
        if (floorInstance != null)
            floorInstance.SetActive(false);
    }

    List<Vector2Int> GenerateOffsets(int radius)
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= radius)
                    offsets.Add(new Vector2Int(x, y));
            }
        }
        return offsets;
    }
}
