using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    
    [SerializeField] private PlayerControllerOld player;
    
    public void StopWalking()
    {
        player.SetMovingState(false);
        Debug.Log("He parado.");
    }
    
    
    
    
}
