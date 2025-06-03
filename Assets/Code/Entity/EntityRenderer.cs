

using UnityEngine;

public class EntityRenderer : MonoBehaviour
{
    [SerializeField] private GridMovementV2 movement;
    
    public float animationSpeed = 1.0f; 

    public Animator animator;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponentInParent<GridMovementV2>();

        if (movement != null)
        {
            //movement.OnMoveComplete += OnMoveAttempt;
            movement.OnMoveFinished += ctx => OnMoveFinished();
        }
        
        if (animator != null)
            animator.speed = animationSpeed;
    }

    private void OnDestroy()
    {
        if (movement != null)
        {
            //movement.OnMoveAttempt -= OnMoveAttempt;
            movement.OnMoveFinished -= ctx => OnMoveFinished();
        }
    }

    private void OnMoveAttempt(bool success)
    {
        if (success && animator != null)
        {
            animator.SetTrigger("walk_trg");
        }
    }

    private void OnMoveFinished()
    {
        if (animator != null)
        {
            animator.ResetTrigger("walk_trg"); // Opcional: puedes cambiarlo por SetBool si usas un parámetro booleano
            //Debug.Log("🔁 Movimiento finalizado visualmente");
        }
    }
}