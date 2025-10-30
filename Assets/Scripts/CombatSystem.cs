using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private GrenadeUIManager grenade;

    public List<GameObject> targets;

    [SerializeField] private GameObject emptyGunShot;
    [SerializeField] private GameObject punchDestroyEffect;
    [SerializeField] private bool aggressive;
    [SerializeField] private bool canDialogue;
    [SerializeField] private Item aggressiveItem;
    public bool combat;

    public Character[] Allies;

    public float radius;

    private float attackTime;
    private float currentTime;

    private float combatXP;

    [SerializeField] private AudioClip whooshClip;

    [HideInInspector] public bool training;
    [HideInInspector] private int trainingHits;

    private Inventory playerInventory;
    [HideInInspector] public LanguageManager languageManager;

    public bool Aggressive
    {
        get
        {
            return aggressive;
        }
        set
        {
            aggressive = value;
        }
    }
    public bool CanDialogue
    {
        get
        {
            return canDialogue;
        }
        set
        {
            canDialogue = value;
        }
    }

    private void Start()
    {
        if (aggressiveItem != null)
        {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().Inventory;
        }
        if(languageManager == null)
        {
            languageManager = GetComponent<HealthSystem>().questSystem.languageManager;
        }
    }

    private void Update()
    {
        if(aggressiveItem != null && aggressive == false)
        {
            foreach (var item in playerInventory.items)
            {
                if(item != null)
                {
                    if(item.itemName == aggressiveItem.name)
                    {
                        aggressive = true;
                    }
                }
            }
        }
        if (aggressive)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward);
            foreach (var hit in hits)
            {
                bool enemy = true;
                foreach (var Ally in Allies)
                {
                    if (Ally.name == hit.collider.name)
                    {
                        enemy = false;
                        break;
                    }
                }

                if (hit.collider.name != gameObject.name && hit.collider.GetComponent<Character>() &&
                    enemy && hit.collider.GetComponent<HealthSystem>().health > 0)
                {
                    Character Character = hit.collider.GetComponent<Character>();

                    if (Character.characterMovement.CurrentRoom == character.characterMovement.CurrentRoom)
                    {
                        if (Character.stealthSystem.stealth)
                        {
                            foreach (var item in Character.combatSystem.targets)
                            {
                                if (item.name == gameObject.name)
                                {
                                    return;
                                }
                            }
                            Character.combatSystem.targets.Add(gameObject);
                        }
                        else
                        {
                            if (canDialogue)
                            {
                                transform.GetChild(3).GetComponent<Person>().Dialogue();
                                Character.characterMovement.StopMovement();
                                Character.characterMovement.interactableTarget = transform.GetChild(3).GetComponent<Interactable>();
                                canDialogue = false;
                            }
                            else
                            {
                                bool find = false;
                                foreach (var item in Character.combatSystem.targets)
                                {
                                    if (item.name == gameObject.name)
                                    {
                                        find = true;
                                        break;
                                    }
                                }
                                if(find == false)
                                {
                                    Character.combatSystem.targets.Add(gameObject);
                                    targets.Add(hit.collider.gameObject);
                                }

                                hit.collider.GetComponent<HealthSystem>().ActiveBodyParts(true);
                                GetComponent<HealthSystem>().ActiveBodyParts(true);

                                if (tag == "NPC")
                                {
                                    foreach (var item in Allies)
                                    {
                                        item.combatSystem.Aggressive = true;
                                        item.combatSystem.CanDialogue = false;
                                        if (GetComponent<CharacterMovement>().CurrentRoom.name == item.characterMovement.CurrentRoom.name)
                                        {
                                            item.combatSystem.radius = 99999;
                                        }
                                    }

                                    if (character.characterMovement.Target == null && character.currentWeapon == null)
                                    {
                                        character.Inventory.TakeEquppedWeapon();
                                    }
                                }
                            }

                            aggressive = false;
                        }
                    }
                }
            }
        }

        character.animator.SetBool("Combat", combat);

        CombatSkillCheck();
    }

    public void StartCombat()
    {
        if(combat == false)
        {
            combat = true;
            if (grenade != null)
            {
                grenade.UpdateButton();
            }
            character.stealthSystem.NotInteractableButton();
        }
    }
    public void Attack()
    {
        if(character.currentWeapon == null)
        {
            int random = Random.Range(0, Mathf.CeilToInt(character.FindSkill(Skills.Unarmed).points / 25));
            character.animator.SetInteger("PunchIndex", random);
            character.animator.SetTrigger("Shot");

            return;
        }

        if(character.currentWeapon.weaponType != WeaponType.AssaultRifle)
        {
            if(character.currentWeapon.weaponType == WeaponType.OneHandMeleeWeapon ||
               character.currentWeapon.weaponType == WeaponType.TwoHandsMeleeWeapon)
            {
                if(character.FindSkill(Skills.MeleeWeapons).points >= 100)
                {
                    character.animator.SetInteger("MeleeWeaponIndex", 2);
                }
                else if (character.FindSkill(Skills.MeleeWeapons).points >= 50)
                {
                    character.animator.SetInteger("MeleeWeaponIndex", 1);
                }
                else
                {
                    character.animator.SetInteger("MeleeWeaponIndex", 0);
                }
            }
            character.animator.SetTrigger("Shot");
        }
        else
        {
            character.animator.SetTrigger("Burst");
        }
    }
    public void Shot()
    {
        bool shot = false;

        if (character.currentWeapon == null)
        {
            HealthSystem healthTarget = Target().GetComponent<HealthSystem>();

            int minDamage = Mathf.CeilToInt(character.FindSkill(Skills.Unarmed).points / 25);

            int maxDamage = Mathf.CeilToInt(character.FindSkill(Skills.Unarmed).points / 25 + (character.Attributes.Strength + character.Attributes.Agility));
            //Unarmed Perk
            if(tag == "Player")
            {
                if (character.PerkSystem.FindPerk(Skills.Unarmed, 0).Active)
                {
                    minDamage += 5;
                }
                if (character.PerkSystem.FindPerk(Skills.Unarmed, 2).Active)
                {
                    maxDamage += 5;
                }
            }

            int random = Random.Range(0, healthTarget.BodyParts.Length);

            float HitChance = 25 + Mathf.CeilToInt(character.FindSkill(Skills.Unarmed).points / 2);

            //Steal Perk
            if (tag == "Player" && character.PerkSystem.FindPerk(Skills.Steal, 1).Active)
            {
                if (character.stealthSystem.light == false ||(tag == "Player" && character.PerkSystem.FindPerk(Skills.Steal, 2).Active))
                {
                    HitChance += 15;
                }
            }

            int rand = Random.Range(1, 101);
            if (rand <= HitChance)
            {
                //Unarmed Perk
                if (tag == "Player" && character.PerkSystem.FindPerk(Skills.Unarmed, 3).Active)
                {
                    int GoinChance = Random.Range(0, 1);
                    if (random == 0)
                    {
                        healthTarget.ApplyDamage(Random.Range(minDamage, maxDamage + 1), healthTarget.BodyParts[2].GetComponent<BodyPart>().body, DamageType.Blunt);
                        Instantiate(punchDestroyEffect, healthTarget.BodyParts[2].bounds.center, punchDestroyEffect.transform.rotation, transform.parent);
                    }
                    else
                    {
                        healthTarget.ApplyDamage(Random.Range(minDamage, maxDamage + 1), healthTarget.BodyParts[random].GetComponent<BodyPart>().body, DamageType.Blunt);
                        Instantiate(punchDestroyEffect, healthTarget.BodyParts[random].bounds.center, punchDestroyEffect.transform.rotation, transform.parent);
                    }
                }
                else
                {
                    healthTarget.ApplyDamage(Random.Range(minDamage, maxDamage + 1), healthTarget.BodyParts[random].GetComponent<BodyPart>().body, DamageType.Blunt);
                    Instantiate(punchDestroyEffect, healthTarget.BodyParts[random].bounds.center, punchDestroyEffect.transform.rotation, transform.parent);
                }
            }
            else
            {
                if(tag == "Player")
                {
                    if (languageManager.currentLanguage == Language.Russian)
                    {
                        EventLog.Print("Промах!", Color.red);
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        EventLog.Print("Miss!", Color.red);
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        EventLog.Print("Nona!", Color.red);
                    }
                }
            }

            if(healthTarget.health > 0)
            {
                healthTarget.GetAudioSource.clip = whooshClip;
                healthTarget.GetAudioSource.Play();
            }

            return;
        }

        if (character.currentWeapon.weaponType == WeaponType.TwoHandsMeleeWeapon ||
           character.currentWeapon.weaponType == WeaponType.OneHandMeleeWeapon)
        {
            shot = true;
        }
        else
        {
            MinusAmmo(character.currentWeapon.ammoName, out shot);
        }

        if (shot)
        {
            if (character.currentWeapon.weaponType == WeaponType.Pistol || character.currentWeapon.weaponType == WeaponType.AssaultRifle ||
                character.currentWeapon.weaponType == WeaponType.SniperRifle || character.currentWeapon.weaponType == WeaponType.Shotgun ||
                character.currentWeapon.weaponType == WeaponType.RocketLauncher)
            {
                character.currentWeapon.SpawnEffect(character.currentWeaponObject.transform.GetChild(0).gameObject);

                if(Target() != null)
                {
                    character.currentWeapon.SpawnBullet(character.currentWeaponObject.transform.GetChild(0).gameObject, Target().GetComponent<HealthSystem>(), character.FindSkill(Skills.Guns));
                    if (character.currentWeapon.weaponType == WeaponType.RocketLauncher)
                    {
                        StartReload();
                    }
                }
            }
            else
            {
                if(Target() != null)
                {
                    HealthSystem healthTarget = Target().GetComponent<HealthSystem>();

                    if (healthTarget.health > 0)
                    {
                        healthTarget.GetAudioSource.clip = whooshClip;
                        healthTarget.GetAudioSource.Play();
                    }

                    character.currentWeapon.SpawnBullet(character.currentWeaponObject.transform.GetChild(0).gameObject, Target().GetComponent<HealthSystem>(), character.FindSkill(Skills.MeleeWeapons));
                }
            }
        }
        else
        {
            EmptyGunShot();
        }
    }

    public void GrenadeAnim()
    {
        grenade.GrenadeButton.interactable = false;
        character.animator.SetTrigger("Grenade");
    }
    public void ActiveGrenadeButton()
    {
        grenade.GrenadeButton.interactable = true;
    }
    public void UseGrenade()
    {
        if (Target() != null)
        {
            Item Grenade = character.Inventory.grenadeSlot.item;

            Grenade.GetComponent<WeaponItem>().SpawnBullet(grenade.spawnPoint, Target().GetComponent<HealthSystem>(), character.FindSkill(Skills.Barter));

            Grenade.number -= 1;
            if (Grenade.number == 0)
            {
                Destroy(Grenade);
                character.Inventory.grenadeSlot.item = null;
            }

            grenade.UpdateButton();
        }
    }

    public void MinusAmmo(string Name, out bool Shot)
    {
        Shot = false;

        foreach (var item in character.Inventory.items)
        {
            if(item != null)
            {
                if (item.itemName == Name)
                {
                    if (item.number > 0)
                    {
                        item.number -= 1;
                        Shot = true;
                        if (item.number == 0)
                        {
                            for (int i = 0; i < character.Inventory.items.Length; i++)
                            {
                                if (character.Inventory.items[i].itemName == item.itemName)
                                {
                                    character.Inventory.items[i] = null;
                                    return;
                                } 
                            }
                        }

                        return;
                    }
                }
            }
        }
    }
    public void EmptyGunShot()
    {
        GameObject effect = Instantiate(emptyGunShot, character.currentWeaponObject.transform.GetChild(0).gameObject.transform);
        Destroy(effect, 2);

        if(tag == "NPC")
        {
            character.Inventory.TakeEquppedWeapon();
        }
    }
    public void StartReload()
    {
        character.currentWeaponObject.transform.GetChild(1).gameObject.SetActive(false);
        foreach (var item in character.Inventory.items)
        {
            if(item != null)
            {
                if (item.itemName == character.currentWeapon.ammoName)
                {
                    character.animator.SetTrigger("Reload");
                    break;
                }
            }
        }
    }
    public void Reload()
    {
        character.currentWeaponObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CombatSkillCheck()
    {
        if (combat)
        {
            if(tag == "Player")
            {
                character.characterMovement.canMove = false;
            }

            if (character.currentWeapon == null)
            {
                character.UpdateWeapon();
                attackTime = 1.5f;
            }
            else
            {
                attackTime = character.currentWeapon.attackTime;
            }

            currentTime += Time.deltaTime;

            if (training)
            {
                if(trainingHits >= 2)
                {
                    currentTime = 0;
                }
            }

            if(currentTime >= attackTime)
            {
                Attack();
                currentTime = 0;
                if (training)
                {
                    trainingHits += 1;
                }
            }
        }
    }

    public GameObject Target()
    {
        GameObject target = null;
        foreach (var item in targets)
        {
            if(item != null)
            {
                target = item.gameObject;
                break;
            }
        }
        return target;
    }

    public void ResetCombat(GameObject gameObject, float xp = 0)
    {
        foreach (var item in targets)
        {
            if (gameObject.name == item.name)
            {
                targets.Remove(item);
                break;
            }
        }

        combatXP += xp;

        if (targets.Count == 0 && combatXP != 0)
        {
            combat = false;
            ExperienceSystem.AddXP(combatXP);
            combatXP = 0;
            character.characterMovement.CanMove();
        }

        grenade.UpdateButton();
        character.stealthSystem.InteractableButton();
    }
}