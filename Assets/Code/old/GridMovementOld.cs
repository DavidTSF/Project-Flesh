using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridMovementOld : MonoBehaviour
{
    
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public float moveSpeed = 2f;

    private Transform movePoint;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        
        movePoint.position = transform.position;
        movePoint.parent = null;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f && isMoving)
            {
                isMoving = false;
                //maskManager.UpdateMasks(movePoint.position);
            }
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        }
    }

    public IEnumerator MoveInDirection(Vector2 direction)
    {
        if (isMoving) yield break;

        if (CanMove(direction))
        {
            isMoving = true;
            if (animator != null) animator.SetTrigger("walk_trg");
            movePoint.position += (Vector3)direction;
            while (Vector3.Distance(transform.position, movePoint.position) > 0.05f)
            {
                yield return null;
            }
            isMoving = false;
        }
        else
        {
            yield return null;
        }
    }

    public bool CanMove(Vector2 direction)
    {
        Vector3 targetWorldPos = movePoint.position + (Vector3)direction;
        Vector3Int gridPosition = groundTilemap.WorldToCell(targetWorldPos);

        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;

        Collider2D[] hitColliders = Physics2D.OverlapPointAll(targetWorldPos);
        foreach (var hit in hitColliders)
        {
            if (!hit.isTrigger && hit.GetComponent<CollidableInteractable>())
                return false;
        }

        return true;
    }
    
    
    
}
