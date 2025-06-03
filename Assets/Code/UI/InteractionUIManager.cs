using System;
using UnityEngine;
using System.Collections.Generic;

public class InteractionUIManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject floorPrefab;

    private GameObject floorInstance;
    
    public float offset = 0.5f;
    private Dictionary<Vector2Int, GameObject> arrows = new();

    public void ShowDirections(Vector3 center, List<Vector2Int> directions)
    {
        Clear();
        
        foreach (var dir in directions)
        {
            Vector3 pos = center + new Vector3(dir.x, dir.y) * offset;
            GameObject arrow = Instantiate(arrowPrefab, pos, Quaternion.identity, transform);
            arrow.transform.up = new Vector3(dir.x, dir.y); // Orientar flecha
            arrows[dir] = arrow;
            Debug.Log("Se han instaciado las flechas de interacción en: " + pos);
        }
    }
    
    public void ShowFloor(Vector3 center)
    {
        if (floorPrefab == null) return;
        
        DestroyFloor();
        
        Vector3 pos = center;
        GameObject floorInstance = Instantiate(floorPrefab, pos, Quaternion.identity, transform);
        floorInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // Ajustar el tamaño del suelo
    }
    
    public void DestroyFloor()
    {
        if (floorInstance != null)
        {
            Destroy(floorInstance);
            floorInstance = null;
        }
    }

    public void Clear()
    {
        foreach (var arrow in arrows.Values)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }
}