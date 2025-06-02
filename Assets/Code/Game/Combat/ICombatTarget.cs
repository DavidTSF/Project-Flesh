using UnityEngine;

public interface ICombatTarget {
    bool CanBeDamaged();
    void TakeDamage(int amount);
    Vector2Int GetGridPosition();
}