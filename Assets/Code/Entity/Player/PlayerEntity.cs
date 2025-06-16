
using UnityEngine;

public class PlayerEntity : MonoBehaviour, ICombatTarget
{
    
    
    public bool CanBeDamaged()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    public Vector2Int GetGridPosition()
    {
        throw new System.NotImplementedException();
    }
}
