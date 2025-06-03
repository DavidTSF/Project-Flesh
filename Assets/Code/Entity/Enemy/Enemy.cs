using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridMovementV2))]
public class Enemy : MonoBehaviour, ICombatTarget
{
    public float moveTime = 0.25f;
    public int attackRange = 1;

    private Transform player;
    private GridMovementV2 gridMovement;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gridMovement = GetComponent<GridMovementV2>();
        EnemyManager.Instance.RegisterEnemy(this);
    }

    public IEnumerator PerformTurn()
    {
        // Esperar a que termine de moverse si ya está en movimiento
        while (gridMovement.IsMoving())
            yield return null;

        if (PlayerInRange())
        {
            yield return AttackPlayer();
        }
        else if (CanSeePlayer())
        {
            yield return MoveTowardsPlayer();
        }
        else
        {
            yield return MoveRandomly();
        }
    }

    private bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private bool CanSeePlayer()
    {
        return Vector2.Distance(transform.position, player.position) <= 5f;
    }

    private IEnumerator AttackPlayer()
    {
        Debug.Log(name + " ataca al jugador!");
        yield return new WaitForSeconds(moveTime);
    }

    private IEnumerator MoveTowardsPlayer()
    {
        Vector2Int direction = GetDirectionTo(player.position);
        bool moved = gridMovement.TryMove(direction);
        if (!moved)
            yield return new WaitForSeconds(moveTime); // simular “acción gastada”
        else
        {
            while (gridMovement.IsMoving())
                yield return null;
        }
    }

    private IEnumerator MoveRandomly()
    {
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        Vector2Int dir = directions[Random.Range(0, directions.Length)];
        bool moved = gridMovement.TryMove(dir);
        if (!moved)
            yield return new WaitForSeconds(moveTime);
        else
        {
            while (gridMovement.IsMoving())
                yield return null;
        }
    }

    private Vector2Int GetDirectionTo(Vector3 targetWorldPos)
    {
        Vector2 delta = targetWorldPos - transform.position;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
    }

    public bool CanBeDamaged()
    {
        return true;
        
    }

    public void TakeDamage(int amount)
    {
        Destroy(gameObject);
    }

    public Vector2Int GetGridPosition()
    {
        return Vector2Int.RoundToInt(gridMovement.WorldToGrid(transform.position));
    }
}
