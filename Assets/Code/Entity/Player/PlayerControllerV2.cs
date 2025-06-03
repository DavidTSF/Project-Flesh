using UnityEngine;

[RequireComponent(typeof(GridMovementV2), typeof(PlayerInteractorGrid))]
public class PlayerControllerV2 : MonoBehaviour, IInteractor
{
    public static PlayerControllerV2 Instance { get; private set; }

    [Header("References")]
    [SerializeField] private VisionMaskManager maskManager;
    [SerializeField] private InteractionUIManager interactionUI;
    [SerializeField] private MenuController menuController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] public CombatSelectorController combatSelector;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private GridMovementV2 gridMovement;
    private PlayerInteractorGrid interactorGrid;

    private bool isInInteractionMode;
    private bool inInventory;
    private bool inCombatMode;
    
    public bool IsBusy() => gridMovement.IsMoving() || isInInteractionMode || inInventory || inCombatMode;

    public Transform GetTransform() => transform;
    public string GetInteractorName() => "Player";
    public bool HasKey(string keyId) => Inventory.Instance.HasKey(keyId);
    public Vector2Int GridPosition => gridMovement.WorldToGrid(transform.position);

#region Unity Events

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gridMovement = GetComponent<GridMovementV2>();
        interactorGrid = GetComponent<PlayerInteractorGrid>();
        inputManager ??= InputManager.Instance;
        


        if (inputManager == null)
        {
            Debug.LogWarning("InputManager no está asignado.");
            return;
        }

        SubscribeInputs();
        combatSelector.OnStopCombatMode += ExitCombatMode;
        gridMovement.OnMoveFinished += OnMoveFinished;
        gridMovement.OnBlockedBySpecialCollider += msg => Debug.Log($"Movimiento bloqueado por: {msg}");

        maskManager?.UpdateMasks(transform.position);
        GenerateDefaultLoadout();
    }

    private void OnDisable()
    {
        if (inputManager == null) return;

        UnsubscribeInputs();
    }

    private void Update()
    {
        if (inputManager == null) return;

        if (inCombatMode)
        {
            //HandleCombatMode();
        }
        if (inputManager.ToggleInventoryRequested)
        {
            inputManager.ResetToggleInventoryFlag();
            ToggleInventory();
            return;
        }

    }

#endregion

#region Input Handling

    private void SubscribeInputs()
    {
        InputManager.Instance.OnMovePressed += HandleMovePressed;
        
        inputManager.OnStartInteraction += OnStartInteraction;
        inputManager.OnInteractDown += OnInteractDown;
        inputManager.OnMenuNavigate += OnMenuNavigate;
        inputManager.OnMenuSubmit += OnMenuSubmit;
        inputManager.OnMenuCancel += OnMenuCancel;
    }

    private void UnsubscribeInputs()
    {
        InputManager.Instance.OnMovePressed -= HandleMovePressed;
        inputManager.OnStartInteraction -= OnStartInteraction;
        inputManager.OnInteractDown -= OnInteractDown;
        inputManager.OnMenuNavigate -= OnMenuNavigate;
        inputManager.OnMenuSubmit -= OnMenuSubmit;
        inputManager.OnMenuCancel -= OnMenuCancel;
    }

    private void HandleMovePressed(Vector2Int dir)
    {
        if (interactorGrid.isSelecting)
        {
            interactorGrid.MoveSelector(dir); 
        }
        if (IsBusy()) return;
        
        if (!gridMovement.IsMoving())
        {
            gridMovement.TryMove(dir);
        }
    }

    private void OnStartInteraction()
    {
        if (inInventory || gridMovement.IsMoving()) return;

        interactorGrid.ToggleInteractionMode();
        isInInteractionMode = interactorGrid.isSelecting;
    }

    private void OnInteractDown()
    {
        if (inInventory || gridMovement.IsMoving())
            return;

        interactorGrid.ConfirmInteraction();
        maskManager?.UpdateMasks(transform.position);
        
        if (interactorGrid.isSelecting)
        {
            interactorGrid.ToggleInteractionMode();
        }
        isInInteractionMode = interactorGrid.isSelecting;
    }


    private void OnMenuNavigate(Vector2 nav)
    {
        if (!inInventory || inCombatMode) return;

        if (Mathf.Abs(nav.x) > 0.1f)
            menuController.OnNavigateHorizontal(nav.x > 0 ? 1 : -1);
        else if (Mathf.Abs(nav.y) > 0.1f)
            menuController.OnNavigateVertical(nav.y > 0 ? -1 : 1);
    }

    private void OnMenuSubmit()
    {
        if (!inInventory || inCombatMode) return;

        if (interactorGrid.isSelecting)
        {
            interactorGrid.ToggleInteractionMode();
            isInInteractionMode = false;
        }

        menuController.Submit();
        UpdateSprite();
    }

    private void OnMenuCancel()
    {
        if (!inInventory || inCombatMode) return;

        if (menuController.IsShowingEquipPopup())
        {
            menuController.CloseEquipDirectionMenu();
            return;
        }

        if (menuController.IsBrowsingItems())
        {
            menuController.Cancel(); 
            return;
        }

        if (!menuController.IsAtRootLevel())
        {
            menuController.Cancel(); 
            return;
        }
        
        CloseInventory();
        menuController.ResetMenu();
    }


    private void OnCombatConfirm()
    {
        if (!inCombatMode) return;
        combatSelector.ConfirmAttack();
    }

    private void OnCombatCancel()
    {
        if (!inCombatMode) return;
        combatSelector.CancelCombatMode();
        ExitCombatMode();
    }

    #endregion

#region Inventory & Combat

    private void ToggleInventory()
    {
        if (inCombatMode)
        {
            Debug.Log("No se puede abrir el inventario en modo combate.");
            return;
        }

        if (inInventory && !menuController.IsShowingEquipPopup())
        {
            CloseInventory();
        }
        else if (!inInventory)
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        if (gridMovement.IsMoving()) return;
        inInventory = true;
        menuController.EnterInventory();
    }

    private void CloseInventory()
    {
        inInventory = false;
        menuController.ExitInventory();
    }

    public void EnterCombatMode() => inCombatMode = true;
    public void ExitCombatMode() => inCombatMode = false;

#endregion

#region Misc

    private void OnMoveFinished(bool success)
    {
        if (success)
            maskManager?.UpdateMasks(transform.position);
    }

    public void UpdateSprite()
    {
        var clothing = Inventory.Instance.currentClothingItem;
        playerSpriteRenderer.sprite = clothing?.sprite;
    }

    private void GenerateDefaultLoadout()
    {
        var rightHand = ScriptableObject.CreateInstance<WeaponSO>();
        rightHand.itemName = "Puño Derecho";
        rightHand.damage = 2;
        rightHand.range = 1f;
        rightHand.category = ItemCategory.Weapon;
        rightHand.description = "Tu puño derecho, siempre listo para la acción.";

        var leftHand = ScriptableObject.CreateInstance<WeaponSO>();
        leftHand.itemName = "Puño Izquierdo";
        leftHand.damage = 2;
        leftHand.range = 1f;
        leftHand.category = ItemCategory.Weapon;
        leftHand.description = "Tu puño izquierdo, siempre listo para la acción.";

        var clothing = Resources.Load<ClothingSO>("ScriptableObjects/PrisonerClothing");

        Inventory.Instance.AddItemSO(rightHand);
        Inventory.Instance.AddItemSO(leftHand);
        Inventory.Instance.AddItemSO(clothing);

        Inventory.Instance.leftHandItem = new InventoryItem(leftHand, 1);
        Inventory.Instance.rightHandItem = new InventoryItem(rightHand, 1);
        Inventory.Instance.currentClothingItem = clothing;

        UpdateSprite();
    }

#endregion
}
