using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public string engCharacterName;
    public string indonesianCharacterName;

    [SerializeField] private GameObject hair;
    [SerializeField] private GameObject beard;
    [SerializeField] private GameObject accessories;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject backpack;

    [SerializeField] private Material[] skins;

    private GameObject[] hairs;
    private GameObject[] beards;
    private GameObject[] _accessories;
    private GameObject[] weapons;
    private GameObject[] backpacks;

    public WeaponItem currentWeapon;
    [HideInInspector] public ArmorItem currentArmor;

    [HideInInspector] public GameObject currentWeaponObject;

    public Animator animator;
    public CharacterMovement characterMovement;
    public StealthSystem stealthSystem;
    public CombatSystem combatSystem;

    [SerializeField] private Skill[] skills;
    [SerializeField] private Attributes attributes;
    [SerializeField] private PerkSystem perkSystem;
    [SerializeField] private Inventory inventory;

    public bool male;
    [HideInInspector] public int currentHairIndex;
    [HideInInspector] public int currentBeardIndex;
    [HideInInspector] public int currentAccessoriesIndex;
    [HideInInspector] public int currentSkinIndex;

    [SerializeField] private GameObject[] maleArmors;
    [SerializeField] private GameObject[] femaleArmors;

    [HideInInspector] public bool hasLeft;

    public GameObject Hair
    {
        get
        {
            return hair;
        }
    }
    public GameObject Beard
    {
        get
        {
            return beard;
        }
    }
    public GameObject Accessories
    {
        get
        {
            return accessories;
        }
    }
    public Skill[] CharacterSkills
    {
        get
        {
            return skills;
        }
        set
        {
            skills = value;
        }
    }
    public Attributes Attributes
    {
        get
        {
            return attributes;
        }
        set
        {
            attributes = value;
        }
    }
    public PerkSystem PerkSystem
    {
        get
        {
            return perkSystem;
        }
    }
    public Inventory Inventory
    {
        get
        {
            return inventory;
        }
    }
    public GameObject[] MaleArmors
    {
        get
        {
            return maleArmors;
        }
    }
    public GameObject[] FemaleArmors
    {
        get
        {
            return femaleArmors;
        }
    }

    public Skill FindSkill(Skills skill)
    {
        Skill Skill = null;
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].skill == skill)
            {
                {
                    Skill = skills[i];
                    break;
                }
            }
        }
        return Skill;
    }

    private void OnEnable()
    {
        if (weapon != null)
        {
            weapons = new GameObject[weapon.transform.childCount];
            for (int i = 0; i < weapon.transform.childCount; i++)
            {
                weapons[i] = weapon.transform.GetChild(i).gameObject;
            }
        }
    }

    public void StartCharacter()
    {
        if (tag == "Player" || tag == "NPC")
        {
            UpdateSkills();
        }
        if(tag == "Player" && characterMovement != null)
        {
            if (characterMovement.CurrentRoom.GetComponent<Room>().radiactive)
            {
                GetComponent<Radiation>().StartRadiation();
            }
        }
    }

    //Screenshot
    //private void Update()
    //{
    //    //Scrennshot
    //    if (Input.GetKeyDown(KeyCode.Space) && tag == "Player")
    //    {
    //        ScreenCapture.CaptureScreenshot("Screenshot.png");
    //    }
    //}

    public void UpdateArmor()
    {
        if(inventory.armorSlot.item != null)
        {
            currentArmor = inventory.armorSlot.item.GetComponent<ArmorItem>();
        }
        else
        {
            currentArmor = null;
        }

        if (inventory != null)
        {
            if (currentArmor == null)
            {
                beard.SetActive(true);
                hair.SetActive(true);

                if (male)
                {
                    maleArmors[0].SetActive(true);
                    for (int i = 1; i < maleArmors.Length; i++)
                    {
                        maleArmors[i].SetActive(false);
                    }
                    for (int i = 0; i < femaleArmors.Length; i++)
                    {
                        femaleArmors[i].SetActive(false);
                    }
                }
                else
                {
                    femaleArmors[0].SetActive(true);
                    for (int i = 1; i < femaleArmors.Length; i++)
                    {
                        femaleArmors[i].SetActive(false);
                    }
                    for (int i = 0; i < maleArmors.Length; i++)
                    {
                        maleArmors[i].SetActive(false);
                    }
                }
            }
            else
            {
                if (male)
                {
                    for (int i = 0; i < maleArmors.Length; i++)
                    {
                        if (maleArmors[i].name == currentArmor.englishItemName)
                        {
                            maleArmors[i].SetActive(true);

                            if (i == 2 || i == 3)
                            {
                                beard.SetActive(false);
                                hair.SetActive(false);
                            }
                            else
                            {
                                beard.SetActive(true);
                                hair.SetActive(true);
                            }
                        }
                        else
                        {
                            maleArmors[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < femaleArmors.Length; i++)
                    {
                        if (femaleArmors[i].name == currentArmor.englishItemName)
                        {
                            femaleArmors[i].SetActive(true);

                            if (i == 2)
                            {
                                beard.SetActive(false);
                                hair.SetActive(false);
                            }
                            else
                            {
                                beard.SetActive(true);
                                hair.SetActive(true);
                            }
                        }
                        else
                        {
                            femaleArmors[i].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void UpdateBackpack()
    {
        if(backpacks != null)
        {
            for (int i = 0; i < backpacks.Length; i++)
            {
                backpacks[i].SetActive(false);
            }
            if (inventory.backpackSlot.item != null)
            {
                for (int i = 0; i < backpacks.Length; i++)
                {
                    if (backpacks[i].name == inventory.backpackSlot.item.englishItemName)
                    {
                        backpacks[i].SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    public void UpdateHair()
    {
        if (hair != null)
        {
            hairs = new GameObject[hair.transform.childCount];
            for (int i = 0; i < hair.transform.childCount; i++)
            {
                hairs[i] = hair.transform.GetChild(i).gameObject;
            }
        }
        if (beard != null)
        {
            beards = new GameObject[beard.transform.childCount];
            for (int i = 0; i < beard.transform.childCount; i++)
            {
                beards[i] = beard.transform.GetChild(i).gameObject;
            }
        }
        if (accessories != null)
        {
            _accessories = new GameObject[accessories.transform.childCount];
            for (int i = 0; i < accessories.transform.childCount; i++)
            {
                _accessories[i] = accessories.transform.GetChild(i).gameObject;
            }
        }
        if (backpack != null)
        {
            backpacks = new GameObject[backpack.transform.childCount];
            for (int i = 0; i < backpack.transform.childCount; i++)
            {
                backpacks[i] = backpack.transform.GetChild(i).gameObject;
            }
        }

        if (currentHairIndex != -1)
        {
            hairs[currentHairIndex].SetActive(true);
        }
        if (currentBeardIndex != -1)
        {
            beards[currentBeardIndex].SetActive(true);
        }

        for (int i = 0; i < maleArmors.Length; i++)
        {
            maleArmors[i].GetComponent<SkinnedMeshRenderer>().material = skins[currentSkinIndex];
        }
        if (femaleArmors[0] == null)
        {
            for (int i = 0; i < femaleArmors.Length; i++)
            {
                femaleArmors[i] = transform.GetChild(5).GetChild(i).gameObject;
            }
        }
        for (int i = 0; i < femaleArmors.Length; i++)
        {
            femaleArmors[i].GetComponent<SkinnedMeshRenderer>().material = skins[currentSkinIndex];
        }
    }

    public void UpdateSkills()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].skill == Skills.Guns)
            {
                skills[i].attributePoints = attributes.Agility * 4;
            }
            else if (skills[i].skill == Skills.Unarmed || skills[i].skill == Skills.MeleeWeapons)
            {
                skills[i].attributePoints = 2 * (attributes.Agility + attributes.Strength);
            }
            else if (skills[i].skill == Skills.Steal)
            {
                skills[i].attributePoints = attributes.Agility;
            }
            else if (skills[i].skill == Skills.Lockpick)
            {
                skills[i].attributePoints = 2 * attributes.Agility;
            }
            else if (skills[i].skill == Skills.Science)
            {
                skills[i].attributePoints = 4 * attributes.Intelligence;
            }
            else if (skills[i].skill == Skills.Repair)
            {
                skills[i].attributePoints = 3 * attributes.Intelligence;
            }
            else if (skills[i].skill == Skills.Doctor)
            {
                skills[i].attributePoints = 2 * attributes.Intelligence;
            }
            else if (skills[i].skill == Skills.Speech)
            {
                skills[i].attributePoints = 5 * attributes.Charisma;
            }
            else if (skills[i].skill == Skills.Barter)
            {
                skills[i].attributePoints = 4 * attributes.Charisma;
            }
        }
    }

    public void UpSkill(Skill skill)
    {
        if (attributes.points != 0 && skill.points < 100)
        {
            skill.newPoints += 1;
            attributes.points -= 1;
        }
    }

    public void CompleteUp()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].lvlPoints += skills[i].newPoints;
            skills[i].newPoints = 0;

            if (i == 0)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(9);
                }
            }
            if (i == 1)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(10);
                }
            }
            if (i == 2)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(11);
                }
            }
            if (i == 3)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(12);
                }
            }
            if (i == 5)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(14);
                }
            }
            if (i == 8)
            {
                if (skills[i].lvlPoints + skills[i].attributePoints >= 100)
                {
                    //googlePlayAchievements.UnlockAchievement(17);
                }
            }
        }

        perkSystem.CheckUnallocatedPerks();
    }

    public void SetWeaponTrigger(bool use = false)
    {
        if (tag != "NPC")
        {
            if (ActiveWeapon() && use)
            {
                animator.SetTrigger("HideWeapon");
            }
            else if (stealthSystem.stealth)
            {
                if (inventory.weaponSlot.item != null)
                {
                    currentWeapon = inventory.weaponSlot.item.GetComponent<WeaponItem>();
                }
                else
                {
                    currentWeapon = null;
                }
            }
            else
            {
                if(inventory.weaponSlot.item != null)
                {
                    currentWeapon = inventory.weaponSlot.item.GetComponent<WeaponItem>();
                }
                else
                {
                    if (!use)
                    {
                        animator.SetTrigger("HideWeapon");
                    }
                    currentWeapon = null;
                }
                UpdateWeapon();
            }
        }
        else
        {
            UpdateWeapon();
        }
    }
    public void UpdateWeapon()
    {
        if (currentWeapon != null)
        {
            animator.SetTrigger(currentWeapon.weaponType.ToString());
        }
        else
        {
            animator.SetTrigger("Unarmed");
        }
    }
    public void ShowWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (currentWeapon.englishItemName == weapons[i].name)
            {
                weapons[i].SetActive(true);
                currentWeaponObject = weapons[i];
            }
            else
            {
                weapons[i].SetActive(false);
            }
        }
    }
    public void HideWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        currentWeaponObject = null;
    }

    private bool ActiveWeapon() 
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}