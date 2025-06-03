using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] public Tilemap collisionTilemap;
    [SerializeField] private int visionRadius = 8;

    private HashSet<Vector3Int> visibleTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> blockers = new HashSet<Vector3Int>();

    // Transformaciones para los 8 octantes del algoritmo de shadowcasting
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

        // Bloqueadores del Tilemap (todas las celdas con tile dentro del rango)
        for (int x = playerCell.x - visionRadius; x <= playerCell.x + visionRadius; x++)
        {
            for (int y = playerCell.y - visionRadius; y <= playerCell.y + visionRadius; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (collisionTilemap.HasTile(cellPos))
                {
                    blockers.Add(cellPos);
                }
            }
        }

        // Bloqueadores de objetos con collider en capa VisionBlocker
        int layerMask = LayerMask.GetMask("VisionBlocker");
        Vector3 worldCenter = collisionTilemap.CellToWorld(playerCell) + collisionTilemap.tileAnchor;
        float range = visionRadius + 1f;
        Collider2D[] allBlockers = Physics2D.OverlapCircleAll(worldCenter, range, layerMask);

        foreach (var collider in allBlockers)
        {
            Vector3 worldPos = collider.bounds.center;
            Vector3Int cellPos = collisionTilemap.WorldToCell(worldPos);
            blockers.Add(cellPos);
        }
    }


    public HashSet<Vector3Int> ComputeFOV(Vector3Int playerCell)
    {
        UpdateBlockers(playerCell);

        visibleTiles.Clear();
        visibleTiles.Add(playerCell); 

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

                // Verificar que está dentro del radio circular
                int dxDist = ax - cx;
                int dyDist = ay - cy;
                if (dxDist * dxDist + dyDist * dyDist <= radius * radius)
                {
                    visibleTiles.Add(tile);
                }

                // SOLO bloquear si la celda está en blockers (tiene collider de visión bloqueadora)
                bool isBlocked = blockers.Contains(tile);

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
