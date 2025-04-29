using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    
    [SerializeField] private PlayerController player;
    
    public void StopWalking()
    {
        player.SetMovingState(false);
        Debug.Log("He parado.");
    }
    
    
    
    
}
