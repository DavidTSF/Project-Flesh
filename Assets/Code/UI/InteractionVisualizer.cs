using System.Collections.Generic;
using UnityEngine;

public class InteractionVisualizer : MonoBehaviour
{
    public GameObject markerPrefab; // Asigna desde el Inspector
    public float offset = 1f;

    private List<GameObject> activeMarkers = new();

    private Vector3[] directions = new Vector3[]
    {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right
    };

    public void ShowMarkers(Vector3 centerPosition)
    {
        HideMarkers();
        
        Vector3 markerPos = centerPosition * offset;
        /*markerPos -= new Vector3(0, 0.3f, 0); ;*/
            
        GameObject marker = Instantiate(markerPrefab, markerPos, Quaternion.identity);
        activeMarkers.Add(marker);
        
        foreach (var dir in directions)
        {
           
            
            /*
            if (dir == Vector3.up) marker.transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (dir == Vector3.right) marker.transform.rotation = Quaternion.Euler(0, 0, -90);
            else if (dir == Vector3.down) marker.transform.rotation = Quaternion.Euler(0, 0, 180);
            else if (dir == Vector3.left) marker.transform.rotation = Quaternion.Euler(0, 0, 90);
            */

            
        }
    }

    public void HideMarkers()
    {
        foreach (var marker in activeMarkers)
        {
            if (marker != null)
                Destroy(marker);
        }
        activeMarkers.Clear();
    }
}