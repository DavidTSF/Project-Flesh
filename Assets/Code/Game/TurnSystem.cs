using System;
using TMPro;
using UnityEngine;

public enum GameState { PLAYER_TURN, ENEMY_TURN, DIED, SWAPPING_LEVEL }

public class TurnSystem : MonoBehaviour
{
    public GameState gameState = GameState.PLAYER_TURN;
    public static TurnSystem Instance;
    
    public int maxActionPoints = 5;
    private int currentActionPoints;

    [SerializeField] private GameObject textMeshPro;

    void Start()
    {
        StartPlayerTurn();
        textMeshPro.GetComponent<TextMeshProUGUI>().text = currentActionPoints.ToString();
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // Solo para debug: pulsa espacio para gastar 1 punto
        if (gameState == GameState.PLAYER_TURN && Input.GetKeyDown(KeyCode.C))
        {
            SpendActionPoint();
        }
    }

    public void SpendActionPoint()
    {
        if (gameState != GameState.PLAYER_TURN)
            return;
        
        currentActionPoints--;

        Debug.Log("Acción realizada. Puntos restantes: " + currentActionPoints);

        if (currentActionPoints <= 0)
        {
            EndPlayerTurn();
        }
        textMeshPro.GetComponent<TextMeshProUGUI>().text = "Puntos de acción: " + currentActionPoints;
    }

    public void StartPlayerTurn()
    {
        Debug.Log("Empieza el turno del jugador");
        gameState = GameState.PLAYER_TURN;
        currentActionPoints = maxActionPoints;
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Fin del turno del jugador");
        gameState = GameState.ENEMY_TURN;
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        Debug.Log("Empieza el turno de los enemigos");

        
        yield return EnemyManager.Instance.EnemyTurn();
        Debug.Log("Fin del turno de los enemigos");
        StartPlayerTurn();
    }
}