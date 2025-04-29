using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] public Tilemap collisionTilemap;
    [SerializeField] private int visionRadius = 8;

    private HashSet<Vector3Int> visibleTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> blockers = new HashSet<Vector3Int>();

    // Transformaciones predefinidas para los octantes
    private static readonly Vector2Int[] transformX = 
    {
        new(1, 0), new(0, 1), new(0, 1), new(-1, 0),
        new(-1, 0), new(0, -1), new(0, -1), new(1, 0)
    };
    private static readonly Vector2Int[] transformY = 
    {
        new(0, 1), new(1, 0), new(-1, 0), new(0, 1),
        new(0, -1), new(-1, 0), new(1, 0), new(0, -1)
    };
    
    
    private void UpdateBlockers(Vector3Int playerCell)
    {
        blockers.Clear();
        
        int layerMask = LayerMask.GetMask("VisionBlocker");
        
        Vector3 worldCenter = collisionTilemap.CellToWorld(playerCell) + collisionTilemap.tileAnchor; 
        
        float range = visionRadius + 1f;  
        
        Collider2D[] allBlockers = Physics2D.OverlapCircleAll(worldCenter, range, layerMask);
        
        foreach (var collider in allBlockers)
        {
            Debug.Log(collider.gameObject.name);
            if (collider.gameObject.layer == LayerMask.NameToLayer("VisionBlocker"))
            {
                // Convertimos las posiciones de los bloqueadores a la celda correspondiente
                Vector3 worldPos = collider.transform.position;
                Vector3Int cellPos = collisionTilemap.WorldToCell(worldPos);

                // Agregamos la celda de bloqueador
                blockers.Add(cellPos);
            }
        }

        // Para debugging, muestra las posiciones de los bloqueadores
        Debug.Log($"Blockers count: {blockers.Count}");
    }




    public HashSet<Vector3Int> ComputeFOV(Vector3Int playerCell)
    {
        UpdateBlockers(playerCell); // Actualizamos los bloqueadores justo antes de calcular FOV

        visibleTiles.Clear();
        visibleTiles.Add(playerCell); // Siempre ves donde estás

        for (int octant = 0; octant < 8; octant++)
        {
            CastLight(playerCell.x, playerCell.y, 1, 1.0f, 0.0f, visionRadius, transformX[octant], transformY[octant]);
        }

        return visibleTiles;
    }

    private void CastLight(int cx, int cy, int row, float startSlope, float endSlope, int radius, Vector2Int transformX, Vector2Int transformY)
    {
        if (startSlope < endSlope) return;

        float nextStartSlope = startSlope;

        for (int i = row; i <= radius; i++)
        {
            bool blocked = false;
            for (int dx = -i, dy = -i; dx <= 0; dx++)
            {
                float lSlope = (dx - 0.5f) / (dy + 0.5f);
                float rSlope = (dx + 0.5f) / (dy - 0.5f);

                if (rSlope > startSlope) continue;
                else if (lSlope < endSlope) break;

                int ax = cx + dx * transformX.x + dy * transformX.y;
                int ay = cy + dx * transformY.x + dy * transformY.y;
                Vector3Int tile = new Vector3Int(ax, ay, 0);

                // Solo si la celda está dentro del rango de visión
                int dxDist = ax - cx;
                int dyDist = ay - cy;
                if (dxDist * dxDist + dyDist * dyDist <= radius * radius)
                {
                    visibleTiles.Add(tile);
                }

                bool isBlocked = collisionTilemap.HasTile(tile) || blockers.Contains(tile);

                if (blocked)
                {
                    if (isBlocked)
                    {
                        nextStartSlope = rSlope;
                        continue;
                    }
                    else
                    {
                        blocked = false;
                        startSlope = nextStartSlope;
                    }
                }
                else
                {
                    if (isBlocked)
                    {
                        blocked = true;
                        nextStartSlope = rSlope;
                        CastLight(cx, cy, i + 1, startSlope, lSlope, radius, transformX, transformY);
                    }
                }
            }
            if (blocked)
            {
                break;
            }
        }
    }

    
    
}
