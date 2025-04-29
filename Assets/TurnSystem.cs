using UnityEngine;

public enum GameState { PLAYER_TURN, ENEMY_TURN, DIED, SWAPING_LEVEL } ;

public class TurnSystem : MonoBehaviour
{
    
    public GameState gameState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = GameState.PLAYER_TURN;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
