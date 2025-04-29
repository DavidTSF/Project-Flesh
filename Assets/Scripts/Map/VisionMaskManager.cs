using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VisionMaskManager : MonoBehaviour
{
    [SerializeField] private GameObject visionMaskPrefab;
    [SerializeField] private Transform visionMaskParent;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private int visionRadius = 4;
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

    // Convertimos la posición mundial del jugador a la celda correspondiente del tilemap
    Vector3Int playerCell = collisionTilemap.WorldToCell(playerPosition);
    Debug.Log("Player position in cells: " + playerCell);

    // Calculamos las celdas visibles usando la función ComputeFOV
    HashSet<Vector3Int> visibleCells = fieldOfView.ComputeFOV(playerCell);

    // Recorremos todas las celdas visibles
    foreach (Vector3Int cell in visibleCells)
    {
        // Verificamos si la celda está dentro de los límites del Tilemap
        if (!collisionTilemap.cellBounds.Contains(cell))
            continue;

        // Convertimos la celda visible a su posición mundial
        Vector3 worldPos = collisionTilemap.GetCellCenterWorld(cell);

        // Aseguramos que solo agregamos máscaras si hay un suelo en esa celda
        if (groundTilemap.HasTile(cell)) // Solo si hay suelo en la celda
        {
            GameObject mask = Instantiate(visionMaskPrefab, worldPos, Quaternion.identity, visionMaskParent);
            currentMasks.Add(mask);

            // Ajustamos el SpriteMask para que se vea correctamente
            SpriteMask spriteMask = mask.GetComponent<SpriteMask>();
            if (spriteMask != null)
            {
                spriteMask.alphaCutoff = 0.2f;
            }

            // Debugging para asegurarnos de que las máscaras se están creando en las posiciones correctas
            Debug.Log("Created mask at: " + worldPos);
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
