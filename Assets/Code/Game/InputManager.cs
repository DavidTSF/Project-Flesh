using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerMovement playerMovement;

    // Flags y eventos públicos para que otros scripts puedan suscribirse
    public bool ToggleInventoryRequested { get; private set; }
    public bool ConfirmPressed { get; set; }
    public bool CancelPressed { get; set; }
    
    public Vector2 MoveInput { get; private set; }
    
    public event System.Action<Vector2Int> OnMovePressed;
    
    public event System.Action OnStartInteraction;
    public event System.Action OnInteractDown;
    public event System.Action<Vector2> OnMenuNavigate;
    public event System.Action OnMenuSubmit;
    public event System.Action OnMenuCancel;
    
    public event System.Action<Vector2Int> OnCombatMove;
    public event System.Action OnCombatConfirm;
    public event System.Action OnCombatCancel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerMovement = new PlayerMovement();

        // Input action bindings
        playerMovement.Player.ToggleInventory.performed += _ => ToggleInventoryRequested = true;
        
        playerMovement.Player.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            MoveInput = input;

            if (input == Vector2.zero) return;

            Vector2Int dir = Mathf.Abs(input.x) > Mathf.Abs(input.y)
                ? (input.x > 0 ? Vector2Int.right : Vector2Int.left)
                : (input.y > 0 ? Vector2Int.up : Vector2Int.down);

            OnMovePressed?.Invoke(dir);
        };
        
        playerMovement.Player.Movement.canceled += _ => MoveInput = Vector2.zero;

        playerMovement.Player.StartInteraction.performed += _ => OnStartInteraction?.Invoke();
        playerMovement.Player.InteractDown.performed += _ => OnInteractDown?.Invoke();

        playerMovement.Menu.Navigate.performed += ctx => OnMenuNavigate?.Invoke(ctx.ReadValue<Vector2>());
        playerMovement.Menu.Submit.performed += _ => OnMenuSubmit?.Invoke();
        playerMovement.Menu.Cancel.performed += _ => OnMenuCancel?.Invoke();
        
        
        // Combat input bindings
        playerMovement.Combat.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            MoveInput = input;

            if (input == Vector2.zero) return;

            Vector2Int dir = Mathf.Abs(input.x) > Mathf.Abs(input.y)
                ? (input.x > 0 ? Vector2Int.right : Vector2Int.left)
                : (input.y > 0 ? Vector2Int.up : Vector2Int.down);

            OnCombatMove?.Invoke(dir);
        };
        
        playerMovement.Combat.Cancel.performed += _ => OnCombatCancel?.Invoke();
        playerMovement.Combat.Submit.performed += _ => OnCombatConfirm?.Invoke();
        
        
        
        //playerMovement.Menu.Submit.performed += _ => OnCombatConfirm?.Invoke();
        //playerMovement.Menu.Cancel.performed += _ => OnCombatCancel?.Invoke();
    }

    private void OnEnable()
    {
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
    }

    public void ResetToggleInventoryFlag()
    {
        ToggleInventoryRequested = false;
    }
}
