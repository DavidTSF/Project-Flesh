using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatSelectorController : MonoBehaviour {
    
    [SerializeField] private GameObject selectorVisual;
    [SerializeField] private GameObject rangeTilePrefab;
    [SerializeField] private GameObject lineTilePrefab;          // Nuevo prefab para línea
    [SerializeField] private Transform rangeContainer;
    [SerializeField] private LayerMask obstacleMask;

    public event System.Action OnStopCombatMode;
    
    InputManager inputManager;
    
    [SerializeField] private GridMovementV2 gridMovement;
    
    private Vector2Int currentInput;
    
    private Vector2Int playerPos;
    private Vector2Int selectorPos;
    private float currentRange = 1f;
    private List<GameObject> rangeTiles = new();
    private List<GameObject> lineTiles = new();                 // Para tiles de la línea
    private bool active = false;

    private bool canAcceptConfirm = false;

    private void Start()
    {
        inputManager ??= InputManager.Instance;
        inputManager.OnCombatMove += UpdateMovement;
        inputManager.OnCombatConfirm += ConfirmAttack;
        inputManager.OnCombatCancel += CancelCombatMode;
        
    }
    
    private void Update()
    {
        if (active && !canAcceptConfirm)
        {
            canAcceptConfirm = true; // se habilita en el siguiente frame
        }
    }
    

    public void EnterCombatMode(Vector2Int startPos, WeaponSO weapon) {
        playerPos = startPos;
        selectorPos = startPos;
        //currentRange = weapon != null ? weapon.range : 1f;
        currentRange = 12f; 
        active = true;
        ShowRange();
        UpdateSelectorVisual();
        DrawLineOfSight(playerPos, selectorPos);
        selectorVisual.SetActive(true);
    }

    private void UpdateMovement(Vector2Int input) {
        if (!active) return;
        
        if (input != Vector2Int.zero) {
            Vector2Int newPos = selectorPos + input;
            if (Vector2Int.Distance(newPos, playerPos) <= currentRange) {
                selectorPos = newPos;
                UpdateSelectorVisual();
                DrawLineOfSight(playerPos, selectorPos);
            }
        }
    }
    

    private void ShowRange() {
        foreach (var go in rangeTiles) Destroy(go);
        rangeTiles.Clear();

        for (int x = -Mathf.CeilToInt(currentRange); x <= Mathf.CeilToInt(currentRange); x++) {
            for (int y = -Mathf.CeilToInt(currentRange); y <= Mathf.CeilToInt(currentRange); y++) {
                Vector2Int pos = playerPos + new Vector2Int(x, y);
                if (Vector2Int.Distance(pos, playerPos) <= currentRange) {
                    var tile = Instantiate(rangeTilePrefab, gridMovement.GridToWorld(pos), Quaternion.identity, rangeContainer);
                    rangeTiles.Add(tile);
                }
            }
        }
    }

    private void UpdateSelectorVisual() {
        selectorVisual.transform.position = gridMovement.GridToWorld(selectorPos);
    }

    public void SetMovementInput(Vector2 input)
    {
        Vector2Int dir = Vector2Int.zero;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            dir = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        else if (Mathf.Abs(input.y) > 0)
            dir = input.y > 0 ? Vector2Int.up : Vector2Int.down;

        currentInput = dir;
    }
    
    private void TryAttackAt(Vector2Int targetPos) {
        
        if (!HasLineOfSight(playerPos, targetPos, obstacleMask)) {
            Debug.Log("No hay línea de visión.");
            return;
        }

        Collider2D target = Physics2D.OverlapPoint(gridMovement.GridToWorld(targetPos));
        
        
        if (target != null && target.TryGetComponent<ICombatTarget>(out var combatTarget)) {
            combatTarget.TakeDamage(1); // Ajusta daño según arma
            Debug.Log("Ataque exitoso a " + target.name);
        } else {
            Debug.Log("No hay objetivo en esa celda.");
        }
        OnStopCombatMode?.Invoke();
        ExitCombatMode();
    }

    private void ExitCombatMode() {
        active = false;
        foreach (var go in rangeTiles) Destroy(go);
        rangeTiles.Clear();
        foreach (var go in lineTiles) Destroy(go);
        lineTiles.Clear();
        selectorVisual.SetActive(false);
    }
    
    private bool HasLineOfSight(Vector2Int from, Vector2Int to, LayerMask obstacleMask)
    {
        // Usa el checker con salida de path (sin usar aquí path)
        return LineOfSightChecker.HasLineOfSight(from, to, gridMovement, obstacleMask, out _);
    }

    private void DrawLineOfSight(Vector2Int from, Vector2Int to)
    {
        foreach (var go in lineTiles) Destroy(go);
        lineTiles.Clear();

        bool hasLoS = LineOfSightChecker.HasLineOfSight(from, to, gridMovement, obstacleMask, out List<Vector2Int> path);

        foreach (var cell in path)
        {
            GameObject tile = Instantiate(lineTilePrefab, gridMovement.GridToWorld(cell), Quaternion.identity, rangeContainer);
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = hasLoS ? Color.green : Color.red;
            lineTiles.Add(tile);
        }
    }
    public void ConfirmAttack()
    {
        if (!active || !canAcceptConfirm) return;
        TryAttackAt(selectorPos);
        canAcceptConfirm = false; // Deshabilitar confirmación hasta el siguiente frame
    }

    public void CancelCombatMode()
    {
        OnStopCombatMode?.Invoke();
        if (!active) return;
        ExitCombatMode();
    }
}

public static class LineOfSightChecker
{
    public static bool HasLineOfSight(Vector2Int from, Vector2Int to, GridMovementV2 grid, LayerMask obstacleMask, out List<Vector2Int> path)
    {
        path = new List<Vector2Int>();

        // Convertimos a posiciones del mundo para el raycast real
        Vector3 fromWorld = grid.GridToWorld(from);
        Vector3 toWorld = grid.GridToWorld(to);

        // Realizamos el raycast entre los dos puntos
        RaycastHit2D hit = Physics2D.Linecast(fromWorld, toWorld, obstacleMask);

        if (hit.collider != null)
        {
            // Línea bloqueada
            Debug.Log($"Line of Sight bloqueada por: {hit.collider.name}");
            return false;
        }

        // Si no hay colisión, construimos la path para dibujarla
        Vector2Int pos = from;

        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        int sx = from.x < to.x ? 1 : -1;
        int sy = from.y < to.y ? 1 : -1;
        int err = dx - dy;

        while (pos != to)
        {
            path.Add(pos);
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                pos.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                pos.y += sy;
            }
        }

        path.Add(to); // Incluir la celda destino
        return true;
    }
}

