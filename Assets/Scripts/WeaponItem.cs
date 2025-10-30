using UnityEngine;
public enum WeaponType
{
    OneHandMeleeWeapon,
    TwoHandsMeleeWeapon,
    Pistol,
    AssaultRifle,
    SniperRifle,
    Shotgun,
    RocketLauncher,
    Grenade
}

public class WeaponItem : Item
{
    public WeaponType weaponType;
    public DamageType damageType;
    public int minDamage;
    public int maxDamage;
    public float distance;
    public float attackTime;
    public string ammoName;
    public GameObject effect;
    public Bullet projectile;

    public void SpawnEffect(GameObject parent)
    {
        if(effect != null)
        {
            GameObject Effect = Instantiate(effect, parent.transform);
            Destroy(Effect, 2);
        }
    }
    public void SpawnBullet(GameObject parent, HealthSystem targetHealth, Skill skill)
    {
        Character character = skill.transform.parent.parent.GetComponent<Character>();

        Bullet bullet = Instantiate(projectile, parent.transform.position, projectile.transform.rotation, targetHealth.transform);
        bullet.transform.SetParent(null, true);
        bullet.damage = Random.Range(minDamage, maxDamage + 1);
        bullet.damageType = damageType;
        bullet.weaponName = englishItemName;

        int StrengthPenalty = 0;
        if(weaponType != WeaponType.Grenade)
        {
            if (needStrength > character.Attributes.Strength)
            {
                StrengthPenalty = needStrength - character.Attributes.Strength;

                //MeleeWeapons Perk
                if (character.tag == "Player")
                {
                    if (character.PerkSystem.FindPerk(Skills.MeleeWeapons, 1).Active)
                    {
                        StrengthPenalty -= 1;
                    }
                }

                if (StrengthPenalty < 0)
                {
                    StrengthPenalty = 0;
                }
            }
        }

        float HitChance = 25 + Mathf.CeilToInt(skill.points / 2) - (20 * StrengthPenalty);

        //Steal Perk
        if (character.tag == "Player" && character.PerkSystem.FindPerk(Skills.Steal, 1).Active)
        {
            if (character.stealthSystem.light == false || character.PerkSystem.FindPerk(Skills.Steal, 2).Active)
            {
                HitChance += 15;
            }
        }

        //Guns Perk
        if (character.tag == "Player" && skill.skill == Skills.Guns)
        {
            if (character.PerkSystem.FindPerk(Skills.Guns, 0).Active)
            {
                HitChance += 5;
            }
            if (character.PerkSystem.FindPerk(Skills.Guns, 1).Active)
            {
                HitChance += 10;
            }
        }
        //MeleeWeapons Perk
        if (character.tag == "Player" && skill.skill == Skills.MeleeWeapons)
        {
            if (character.PerkSystem.FindPerk(Skills.MeleeWeapons, 0).Active)
            {
                HitChance += 5;
            }
        }

        if (HitChance > 100)
        {
            HitChance = 100;
        }
        else if(HitChance < 0)
        {
            HitChance = 0;
        }

        if (weaponType == WeaponType.RocketLauncher || weaponType == WeaponType.Grenade || englishItemName == "Flamethrower")
        {
            for (int i = 0; i < targetHealth.BodyParts.Length; i++)
            {
                if (targetHealth.BodyParts[i].GetComponent<BodyPart>().body == Body.Torso)
                {
                    if(weaponType == WeaponType.Grenade)
                    {
                        HitChance = 100;
                    }
                    else if (weaponType == WeaponType.RocketLauncher || englishItemName == "Flamethrower")
                    {
                        HitChance = 100 - (20 * StrengthPenalty);
                    }
                    if (CheckChance(HitChance) == false)
                    {
                        bullet.GetComponent<Collider>().enabled = false;
                        bullet.visualDestroy = true;
                    }
                    bullet.alwaysTarget = targetHealth.BodyParts[i].gameObject;
                    break;
                }
            }
        }
        else
        {
            if (CheckChance(HitChance) == false)
            {
                bullet.GetComponent<Collider>().enabled = false;
                bullet.alwaysTarget = null;
                bullet.target = targetHealth.BodyParts[1].bounds.center;

                if (character.tag == "Player")
                {
                    if(character.combatSystem.languageManager.currentLanguage == Language.Russian)
                    {
                        EventLog.Print("Промах!", Color.red);
                    }
                    else if (character.combatSystem.languageManager.currentLanguage == Language.English)
                    {
                        EventLog.Print("Miss!", Color.red);
                    }
                    else if (character.combatSystem.languageManager.currentLanguage == Language.Indonesian)
                    {
                        EventLog.Print("Nona!", Color.red);
                    }
                }
                return;
            }

            //Guns Perk
            if (character.tag == "Player" && skill.skill == Skills.Guns)
            {
                if (character.PerkSystem.FindPerk(Skills.Guns, 2).Active)
                {
                    int headChance = Random.Range(0, 101);
                    if (headChance <= 35)
                    {
                        bullet.alwaysTarget = targetHealth.BodyParts[0].gameObject;
                    }
                    else
                    {
                        int randomChance = Random.Range(1, targetHealth.BodyParts.Length);
                        bullet.alwaysTarget = targetHealth.BodyParts[randomChance].gameObject;
                    }
                    return;
                }
            }
            int random = Random.Range(0, targetHealth.BodyParts.Length);
            bullet.alwaysTarget = targetHealth.BodyParts[random].gameObject;
        }
    }

    bool CheckChance(float hitChance)
    {
        int rand = Random.Range(1, 101);
        if (rand <= hitChance)
        {
            return true;
        }
        return false;
    }
}