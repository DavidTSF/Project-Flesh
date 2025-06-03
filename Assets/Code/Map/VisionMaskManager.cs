using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VisionMaskManager : MonoBehaviour
{
    [SerializeField] private GameObject visionMaskPrefab;
    [SerializeField] private Transform visionMaskParent;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private int visionRadius;
    [SerializeField] private Tilemap groundTilemap;
    
    [SerializeField] private FieldOfView fieldOfView;
    private List<GameObject> currentMasks = new List<GameObject>();

    private void Awake()
    {
        
        if (fieldOfView == null)
        {
            Debug.LogError("FieldOfView no encontrado en la escena. Asegúrate de añadirlo a un GameObject.");
        }
    }

public void UpdateMasks(Vector3 playerPosition)
{
    if (fieldOfView == null)
        return;

    ClearMasks();
    
    Vector3Int playerCell = collisionTilemap.WorldToCell(playerPosition);
    HashSet<Vector3Int> visibleCells = fieldOfView.ComputeFOV(playerCell);

    // Recorremos todas las celdas visibles
    foreach (Vector3Int cell in visibleCells)
    {
        // Verificamos si la celda está dentro de los límites del Tilemap
        if (!collisionTilemap.cellBounds.Contains(cell))
            continue;
        Vector3 worldPos = collisionTilemap.GetCellCenterWorld(cell);
        if (groundTilemap.HasTile(cell)) 
        {
            GameObject mask = Instantiate(visionMaskPrefab, worldPos, Quaternion.identity, visionMaskParent);
            currentMasks.Add(mask);
            
            

            // Ajustamos el SpriteMask para que se vea correctamente
            SpriteMask spriteMask = mask.GetComponent<SpriteMask>();
            if (spriteMask != null)
            {
                spriteMask.alphaCutoff = 0.2f;
            }
        }
    }
}


    private void ClearMasks()
    {
        foreach (GameObject mask in currentMasks)
        {
            if (mask != null)
                Destroy(mask);
        }
        currentMasks.Clear();
    }
}
