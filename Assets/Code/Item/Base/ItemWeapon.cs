
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemWeapon : Item
{
    public int damage;
    public float range;

    public ItemWeapon(string name, string description, ItemCategory category, int damage, float range)
        : base(name, description, category)
    {
        this.damage = damage;
        this.range = range;
    }
    
}