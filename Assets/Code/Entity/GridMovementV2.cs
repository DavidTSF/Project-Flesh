using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridMovementV2 : MonoBehaviour
{
    public event Action<bool> OnMoveFinished;
    
    public event Action<string> OnBlockedBySpecialCollider;
    
    private Coroutine moveCoroutine;
    
    [Header("Settings")]
    public float moveDuration = 0.2f;
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;

    public Vector2Int currentGridPos;
    private Vector2Int targetGridPos;
    private bool isMoving = false;
    private Animator animator;

    private void Start()
    {
        currentGridPos = WorldToGrid(transform.position);
        transform.position = GridToWorld(currentGridPos);
        animator = GetComponentInChildren<Animator>();
    }

    public bool TryMove(Vector2Int direction)
    {
        if (isMoving) return false;

        Vector2Int newGridPos = currentGridPos + direction;

        if (TilemapBlocked(newGridPos))
        {
            OnMoveFinished?.Invoke(false);
            return false;
        }

        targetGridPos = newGridPos;
        moveCoroutine = StartCoroutine(MoveToTarget(direction));
        return true;
    }

    private IEnumerator MoveToTarget(Vector2Int direction)
    {
        isMoving = true;

        animator?.SetTrigger("walk_trg");

        Vector3 start = transform.position;
        Vector3 end = GridToWorld(targetGridPos);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        currentGridPos = targetGridPos;
        isMoving = false;
        OnMoveFinished?.Invoke(true);
    }

    public bool IsMoving() => isMoving;

    private bool TilemapBlocked(Vector2Int gridPos)
    {
        if (!groundTilemap || !collisionTilemap)
            return false;

        if (!groundTilemap.HasTile((Vector3Int)gridPos)) return true;
        if (collisionTilemap.HasTile((Vector3Int)gridPos)) return true;

        Vector3 worldPos = GridToWorld(gridPos);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        foreach (var hit in hits)
        {
            if (!hit.isTrigger && hit.enabled)
            {
                var collidable = hit.GetComponent<CollidableInteractable>();
                if (collidable != null)
                {
                    OnBlockedBySpecialCollider?.Invoke("Blocked by: " + collidable.name);
                    return true;
                }
                return true;
            }
        }
        return false;
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return (Vector2Int)groundTilemap.WorldToCell(worldPos);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return groundTilemap.GetCellCenterWorld((Vector3Int)gridPos);
    }
}
