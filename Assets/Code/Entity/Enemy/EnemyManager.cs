using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<Enemy> enemies = new List<Enemy>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("Iniciando turno de enemigos...");

        foreach (Enemy enemy in enemies)
        {
            yield return enemy.PerformTurn();
        }

        Debug.Log("Turno de enemigos terminado");
        TurnSystem.Instance.StartPlayerTurn(); // Llama al TurnSystem para devolver el turno
    }
}