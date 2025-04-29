using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    public GameObject floatingMessagePrefab;
    public Transform messageSpawnPoint;
    
    [SerializeField] private GameObject spriteRenderer;
    private Animator animator;
    public Transform movePoint;
    public float moveSpeed = 2f;
    [SerializeField] private VisionMaskManager maskManager;

    private bool isChoosingInteractionDirection = false;
    private Vector2 selectedInteractionDirection = Vector2.zero;
    public InteractionVisualizer visualizer;

    private bool isMoving = false;

    private void Start()
    {
        maskManager.UpdateMasks(transform.position);
        movePoint.parent = null;
        animator = spriteRenderer.GetComponent<Animator>();
        playerMovement.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerMovement.Main.StartInteraction.performed += ctx => EnterInteractionMode();
        playerMovement.Main.ChooseDirection.performed += ctx => ChooseDirection(ctx.ReadValue<Vector2>());
        playerMovement.Main.InteractDown.performed += ctx => TryInteractDown();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, movePoint.position) < 0.05f && isMoving)
        {
            isMoving = false;
            maskManager.UpdateMasks(movePoint.position);
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void EnterInteractionMode()
    {
        if (!isMoving)
        {
            isChoosingInteractionDirection = true;
            visualizer.ShowMarkers(transform.position);
            Debug.Log("Modo de interacción activado. Usa flechas o ENTER.");
        }
    }

    private void ChooseDirection(Vector2 direction)
    {
        if (!isChoosingInteractionDirection)
            return;

        selectedInteractionDirection = direction.normalized;
        Vector3 targetPos = movePoint.position + (Vector3)selectedInteractionDirection;

        TryInteractWithObjectAt(targetPos);
        isChoosingInteractionDirection = false;
        visualizer.HideMarkers();
    }

    private void TryInteractWithObjectAt(Vector3 targetPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(targetPos);
        if (hit != null)
        {
            var interactable = hit.GetComponent<Interactable>();
            if (interactable != null )
            {
                interactable.Interact();
                maskManager.UpdateMasks(movePoint.position);
                ShowFloatingMessage(interactable.interactTip);
                Debug.Log("¡Interacción realizada!");
            }
        }
        else
        {
            Debug.Log("Nada que interactuar en esa dirección.");
        }
    }

    private void TryInteractDown()
    {
        if (isChoosingInteractionDirection)
        {
            Debug.Log("Intentando interactuar Centro...");
            Vector3 worldPos = movePoint.position;
            TryInteractWithObjectAt(worldPos);
        }
    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction) && !isMoving)
        {
            isMoving = true;
            animator.SetTrigger("walk_trg");
            movePoint.position += (Vector3)direction;
        }
    }

    private bool CanMove(Vector2 direction)
    {
        if (isChoosingInteractionDirection) return false;

        Vector3 targetWorldPos = movePoint.position + (Vector3)direction;
        Vector3Int gridPosition = groundTilemap.WorldToCell(targetWorldPos);

        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;

        // Verificar colisiones físicas solo si el tile es transitable
        return !IsBlockedByCollider(targetWorldPos);
    }
    
    private bool IsBlockedByCollider(Vector3 targetPos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(targetPos);
        foreach (var hit in hitColliders)
        {
            var collidable = hit.GetComponent<CollidableInteractable>();
            if (collidable != null && hit is Collider2D col && !col.isTrigger && col.enabled)
            {
                ShowFloatingMessage("Está cerrada...");
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        playerMovement = new PlayerMovement();
    }

    private void OnEnable()
    {
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
    }

    private void ShowFloatingMessage(string message)
    {
        if (floatingMessagePrefab == null || messageSpawnPoint == null) return;

        GameObject canvas = GameObject.Find("Canvas");

        // Convertimos la posición del spawn a viewport
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(messageSpawnPoint.position);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 screenPosition = new Vector2(
            (viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
        );
        screenPosition.y += 50f; // Ajuste de altura

        GameObject messageGO = Instantiate(floatingMessagePrefab, canvas.transform);
        RectTransform messageRect = messageGO.GetComponent<RectTransform>();
        messageRect.anchoredPosition = screenPosition;

        var floatMsg = messageGO.GetComponent<FloatingMessage>();
        floatMsg.SetText(message);
    }

    public void SetMovingState(bool isMoving)
    {
        this.isMoving = isMoving;
    }
    
}
