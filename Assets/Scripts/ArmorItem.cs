using UnityEngine;
public enum DamageType
{
    Blunt,
    Bullet,
    Explosion,
    Ignite,
    Radiation
}

public class ArmorItem : Item
{
    public DamageType[] damageTypes;

    public int[] protection;
}
