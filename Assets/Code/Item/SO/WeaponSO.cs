using UnityEngine;

public enum WeaponType {
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
public class WeaponSO : ItemSO
{
    public int damage;
    public float range = 1f;
    public Sprite sprite;
    public WeaponType weaponType = WeaponType.Melee;
    
    [Header("Ranged only")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    public WeaponSO(string itemName, string description, ItemCategory category, Sprite icon, int damage, float range) : base(itemName, description, category, icon)
    {
        this.damage = damage;
        this.range = range;
    }
}