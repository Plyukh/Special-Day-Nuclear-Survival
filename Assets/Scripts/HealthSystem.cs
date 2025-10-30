using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private Button healButton;
    [SerializeField] private Sprite doctorSprite;

    [SerializeField] private Character character;
    [SerializeField] private Container container;
    [SerializeField] private DamageType[] damageTypes;
    public int[] protection;

    public Slider hpSlider;
    private Slider dopHpSlider;

    public float health;
    public float maxHealth;
    public float xp;

    [SerializeField] private Collider[] bodyParts;
    [SerializeField] private GameObject[] bodyDeathPrefabs;
    [SerializeField] private GameObject bodyEffect;

    private int heal;
    private int healRad;

    private GameObject skins;

    private AudioSource audioSource;

    [SerializeField] SceneManagerScript sceneManagerScript;
    public QuestSystem questSystem;
    [SerializeField] Settings settings;
    [SerializeField] InteractableUI interactableUI;

    [SerializeField] AudioClip[] painClips;
    [SerializeField] AudioClip[] deathClips;

    [SerializeField] AudioClip[] playerFemalePainClips;
    [SerializeField] AudioClip[] playerFemaleDeathClips;

    [SerializeField] private PlayerDeath playerDeath;

    [SerializeField] private GameObject bloodPrefab;

    public Collider[] BodyParts
    {
        get
        {
            return bodyParts;
        }
    }

    public AudioSource GetAudioSource
    {
        get
        {
            return audioSource;
        }
    }

    public void StartHealth()
    {
        audioSource = GetComponent<AudioSource>();

        if(tag == "Player")
        {
            UpdatePerks(Skills.Unarmed);
            UpdatePerks(Skills.Doctor);
            UpdateMaxHealth();
        }
        else if(tag == "NPC")
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).name == "Skins")
                {
                    skins = transform.GetChild(i).gameObject;
                    break;
                }
            }

            if(health <= 0)
            {
                Death(false);
            }
        }
    }

    private void Update()
    {
        if(tag == "NPC" && hpSlider != null && hpSlider.gameObject.activeInHierarchy)
        {
            RectTransform rect = hpSlider.transform.parent.parent.parent.GetComponent<RectTransform>();
            rect.position = Camera.main.WorldToScreenPoint(gameObject.transform.GetChild(0).position);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
        }
    }

    public void ApplyDamage(float Damage, Body BodyPart, DamageType damageType, string weaponName = "")
    {
        if (character.currentArmor != null)
        {
            for (int i = 0; i < character.currentArmor.damageTypes.Length; i++)
            {
                if (damageType == character.currentArmor.damageTypes[i])
                {
                    float block = Damage / 100 * character.currentArmor.protection[i] + protection[i];
                    Damage -= block;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < damageTypes.Length; i++)
            {
                if (damageType == damageTypes[i])
                {
                    float block = Damage / 100 * protection[i];
                    Damage -= block;
                    break;
                }
            }
        }

        if(Damage < 0)
        {
            Damage = 0;
        }

        if(BodyPart == Body.Head)
        {
            Damage *= 2;
            character.animator.SetTrigger("Hit");

            if(tag == "Player")
            {
                if (character.male)
                {
                    RandomSound(painClips);
                }
                else
                {
                    RandomSound(playerFemalePainClips);
                }
            }
            else
            {
                RandomSound(painClips);
            }
        }
        else if (BodyPart == Body.Groin)
        {
            if (character.male)
            {
                Damage *= 1.75f;
            }
            else
            {
                Damage *= 1.25f;
            }
            character.animator.SetTrigger("Hit");

            if (tag == "Player")
            {
                if (character.male)
                {
                    RandomSound(painClips);
                }
                else
                {
                    RandomSound(playerFemalePainClips);
                }
            }
            else
            {
                RandomSound(painClips);
            }
        }

        health -= Mathf.Ceil(Damage);

        if (hpSlider.gameObject.activeInHierarchy)
        {
            UpdateSlider();
        }

        if (health <= 0)
        {
            SetDeathBody(BodyPart, weaponName);
            Death();
        }
    }

    private void SetDeathBody(Body bodyPart, string weaponName)
    {
        int index = -1;
        if (bodyPart == Body.Head)
        {
            if(weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw" || weaponName == "Crowbar" || weaponName == "Fire Axe" || weaponName == "Rebar Club")
            {
                index = 0;
                RemoveHair();

                for (int i = 0; i < sceneManagerScript.characters.Length; i++)
                {
                    if(sceneManagerScript.characters[i].name == gameObject.name)
                    {
                        sceneManagerScript.deathHair[i] = true;
                        break;
                    }
                }
            }
        }
        else if (bodyPart == Body.Torso || bodyPart == Body.Groin)
        {
            if (weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw")
            {
                index = 1;
            }
        }
        else if (bodyPart == Body.RightHand)
        {
            if (weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw" || weaponName == "Fire Axe")
            {
                index = 2;
            }
        }
        else if (bodyPart == Body.LeftHand)
        {
            if (weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw" || weaponName == "Fire Axe")
            {
                index = 3;
            }
        }
        else if (bodyPart == Body.RightLeg)
        {
            if (weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw" || weaponName == "Fire Axe")
            {
                index = 4;
            }
        }
        else if (bodyPart == Body.LeftLeg)
        {
            if (weaponName == "Wasteland Rebel" || weaponName == "Assault Rifle" || weaponName == "Hunting Rifle" ||
               weaponName == "Hunting Shotgun" || weaponName == "Lever Action Shotgun" || weaponName == "Revolver" ||
               weaponName == "Chainsaw" || weaponName == "Fire Axe")
            {
                index = 5;
            }
        }
        else if(weaponName == "Molotov" || weaponName == "Flamethrower")
        {
            index = 6;
        }
        else if (weaponName == "Grenade" || weaponName == "Rocket Launcher")
        {
            index = 7;
        }

        if (index >= 0)
        {
            audioSource.pitch = 1;

            if(tag == "NPC")
            {
                for (int i = 0; i < skins.transform.childCount; i++)
                {
                    if (skins.transform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        for (int j = 0; j < bodyDeathPrefabs[index].transform.childCount; j++)
                        {
                            if (skins.transform.GetChild(i).name == bodyDeathPrefabs[index].transform.GetChild(j).name)
                            {
                                skins.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().sharedMesh = bodyDeathPrefabs[index].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().sharedMesh;
                                Instantiate(bodyEffect, bodyParts[index].bounds.center, bodyEffect.transform.rotation, gameObject.transform);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    public void RemoveHair()
    {
        character.Hair.SetActive(false);
        character.Beard.SetActive(false);
        character.Accessories.SetActive(false);
        if (character.Hair.transform.parent.GetChild(4).gameObject.activeInHierarchy)
        {
            character.Hair.transform.parent.GetChild(4).gameObject.SetActive(false);
        }
    }

    public void UseMedKit()
    {
        character.Inventory.medicineSlot.item.GetComponent<MedkitItem>().UseMedTik(character);
    }
    public void ApplyHeal(int Heal)
    {
        healButton.interactable = false;

        character.SetWeaponTrigger(true);
        character.animator.SetTrigger("Heal");

        heal = Heal;

        audioSource.clip = character.Inventory.medicineSlot.item.GetComponent<MedkitItem>().audioClip;
        audioSource.Play();
    }
    public void ApplyHealRad(int HealRad)
    {
        healButton.interactable = false;

        character.SetWeaponTrigger(true);
        character.animator.SetTrigger("Heal");

        healRad = HealRad;
    }
    public void Heal()
    {
        if(heal > 0)
        {
            health += heal;
            if (health > maxHealth)
            {
                health = maxHealth;
            }

            if (character.combatSystem.languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print(character.characterName + " восстанавливает здоровье на " + heal, Color.green);
            }
            else if (character.combatSystem.languageManager.currentLanguage == Language.English)
            {
                EventLog.Print(character.characterName + " restores health by " + heal, Color.green);
            }
            else if (character.combatSystem.languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print(character.characterName + " memulihkan kesehatan dengan " + heal, Color.green);
            }
        }
        else if(healRad > 0)
        {
            GetComponent<Radiation>().currentRad -= healRad;
            if (GetComponent<Radiation>().currentRad < 0)
            {
                GetComponent<Radiation>().currentRad = 0;
            }

            if (character.combatSystem.languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print(character.characterName + " вывел радиацию на " + healRad, Color.green);
            }
            else if (character.combatSystem.languageManager.currentLanguage == Language.English)
            {
                EventLog.Print(character.characterName + " increased the radiation by " + healRad, Color.green);
            }
            else if (character.combatSystem.languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print(character.characterName + " meningkatkan radiasi sebesar " + healRad, Color.green);
            }
        }

        heal = -1;
        healRad = -1;

        MedkitItem medkitItem = character.Inventory.medicineSlot.item.GetComponent<MedkitItem>();

        audioSource.clip = medkitItem.audioClip;
        audioSource.Play();

        if (medkitItem.number == 0)
        {
            Destroy(character.Inventory.medicineSlot.item);
            character.Inventory.medicineSlot.item = null;
        }
        character.Inventory.medicineSlot.UpdateStot();

        UpdateButton();
        UpdateSlider();
    }
    public void SexHeal()
    {
        health = maxHealth;

        if (character.combatSystem.languageManager.currentLanguage == Language.Russian)
        {
            EventLog.Print(character.characterName + " восстанавливает здоровье", Color.green);
        }
        else if (character.combatSystem.languageManager.currentLanguage == Language.English)
        {
            EventLog.Print(character.characterName + " restores health", Color.green);
        }
        else if (character.combatSystem.languageManager.currentLanguage == Language.Indonesian)
        {
            EventLog.Print(character.characterName + " memulihkan kesehatan", Color.green);
        }

        heal = -1;

        UpdateButton();
        UpdateSlider();
    }

    public void Death(bool sound = true)
    {
        if (sound)
        {
            if (tag == "Player")
            {
                if (character.male)
                {
                    RandomSound(deathClips);
                }
                else
                {
                    RandomSound(playerFemaleDeathClips);
                }
            }
            else
            {
                RandomSound(deathClips);
            }
        }

        character.animator.SetInteger("DeathIndex", Random.Range(0, 11));
        character.animator.SetTrigger("Death");
        character.animator.SetBool("Walk", false);
        character.animator.SetBool("Running", false);

        if (tag == "NPC")
        {
            if (sound)
            {
                if(character.combatSystem.Target() != null)
                {
                    character.combatSystem.Target().GetComponent<CombatSystem>().ResetCombat(gameObject, xp);
                }
            }

            character.characterMovement.DisabledNavMeshAgent();
            character.characterMovement.enabled = false;
            character.combatSystem.enabled = false;

            container.clickCollider.enabled = true;
            gameObject.GetComponent<Collider>().enabled = false;

            character.HideWeapon();

            ActiveBodyParts(false);
            ActiveOutline();

            if(hpSlider != null && hpSlider.gameObject.activeInHierarchy)
            {
                Destroy(hpSlider.transform.parent.parent.gameObject);
            }

            for (int i = 0; i < questSystem.quests.Length; i++)
            {
                for (int j = 0; j < questSystem.quests[i].QuestParts.Length; j++)
                {
                    if (questSystem.quests[i].QuestParts[j].QuestType == QuestType.Murder)
                    {
                        if(questSystem.quests[i].QuestParts[j].characterName == character.characterName)
                        {
                            questSystem.CompletePart(i,j);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var item in sceneManagerScript.characters)
            {
                var combatSystem = item.GetComponent<CombatSystem>();
                if (combatSystem != null)
                {
                    combatSystem.targets.Clear();
                    combatSystem.combat = false;
                }
            }

            Camera.main.GetComponent<CameraZoom>().OnPointerObject();
            settings.music.volume = 0;
            playerDeath.window.SetActive(true);
            playerDeath.RandomText(character);
        }
    }

    public void ActiveBodyParts(bool SetActive)
    {
        foreach (var item in bodyParts)
        {
            item.enabled = SetActive;
        }
    }

    public void ActiveOutline()
    {
        for (int i = 0; i < skins.transform.childCount; i++)
        {
            skins.transform.GetChild(i).GetComponent<Outline>().OutlineWidth = 2;
        }
    }
    public void ActiveSteathOutline(bool value)
    {
        if (value)
        {
            for (int i = 0; i < skins.transform.childCount; i++)
            {
                skins.transform.GetChild(i).GetComponent<Outline>().OutlineColor = Color.red;
                skins.transform.GetChild(i).GetComponent<Outline>().OutlineWidth = 2;
            }
        }
        else
        {
            for (int i = 0; i < skins.transform.childCount; i++)
            {
                skins.transform.GetChild(i).GetComponent<Outline>().OutlineColor = Color.white;
                skins.transform.GetChild(i).GetComponent<Outline>().OutlineWidth = 0;
            }
        }
    }

    private void UpdateSlider()
    {
        if(tag == "NPC")
        {
            hpSlider.maxValue = maxHealth / 2;
            hpSlider.value = health / 2;
            dopHpSlider.maxValue = maxHealth / 2;
            dopHpSlider.value = health / 2;

            hpSlider.transform.parent.parent.GetChild(2).GetComponent<Text>().text = health + "/" + maxHealth;
        }
        else
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = health;

            hpSlider.transform.GetChild(1).GetComponent<Text>().text = health + "/" + maxHealth;
        }
    }
    public void UpdateButton()
    {
        if(character.Inventory.medicineSlot.item != null)
        {
            healButton.interactable = true;
            if (character.Inventory.medicineSlot.item.itemSprite != null)
            {
                healButton.transform.GetChild(1).GetComponent<Image>().sprite = character.Inventory.medicineSlot.item.itemSprite;
            }
            else
            {
                for (int i = 0; i < character.Inventory.medicineSlot.ItemPrefabs.transform.childCount; i++)
                {
                    Item item = character.Inventory.medicineSlot.ItemPrefabs.transform.GetChild(i).GetComponent<Item>();
                    if (item.itemName == character.Inventory.medicineSlot.item.itemName)
                    {
                        healButton.transform.GetChild(1).GetComponent<Image>().sprite = item.itemSprite;
                        break;
                    }
                }
            }
            healButton.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = character.Inventory.medicineSlot.item.number.ToString();
        }
        else
        {
            healButton.interactable = false;
            healButton.transform.GetChild(1).GetComponent<Image>().sprite = doctorSprite;
            healButton.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "0";
        }
    }
    public void UpdatePerks(Skills skill)
    {
        //Protection Perk
        if (character.PerkSystem.FindPerk(Skills.Unarmed, 1).Active && skill == Skills.Unarmed)
        {
            protection[0] += 5;
            protection[1] += 5;
            protection[3] += 5;
        }
        if (character.PerkSystem.FindPerk(Skills.Doctor, 1).Active && skill == Skills.Doctor)
        {
            for (int i = 0; i < protection.Length; i++)
            {
                protection[i] += 5;
            }
        }
    }
    public void UpdateMaxHealth()
    {
        //Health Perk
        if (character.PerkSystem.FindPerk(Skills.Doctor, 0).Active)
        {
            maxHealth = 25 + character.Attributes.Strength * 4 + ExperienceSystem.lvl * 3;
        }
        else
        {
            maxHealth = 15 + character.Attributes.Strength * 4 + ExperienceSystem.lvl * 3;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateSlider();
    }

    public void SpawnSlider()
    {
        if(hpSlider.gameObject.activeInHierarchy == false)
        {
            Slider slider = Instantiate(hpSlider, interactableUI.gameObject.transform);
            hpSlider = slider.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
            dopHpSlider = slider.transform.GetChild(1).GetChild(1).GetComponent<Slider>();
            UpdateSlider();
        }
    }

    public void SpawnBlood()
    {
        GameObject blood = Instantiate(bloodPrefab, bodyParts[1].transform);
        blood.transform.SetParent(null);
        blood.transform.localEulerAngles = new Vector3(0, 0, 0);
        blood.transform.position = new Vector3(blood.transform.position.x, gameObject.transform.position.y + 0.08f, blood.transform.position.z);
    }

    private void RandomSound(AudioClip[] clips)
    {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.Play();
    }
}

[System.Serializable]
public class PlayerDeath
{
    public GameObject window;
    public Text deathText;
    public string[] texts;
    public string[] engTexts;
    public string[] indonesianTexts;

    public void RandomText(Character character)
    {
        int random = Random.Range(0, texts.Length);
        if (character.combatSystem.languageManager.currentLanguage == Language.Russian)
        {
            deathText.text = texts[random];
        }
        else if (character.combatSystem.languageManager.currentLanguage == Language.English)
        {
            deathText.text = engTexts[random];
        }
        else if (character.combatSystem.languageManager.currentLanguage == Language.Indonesian)
        {
            deathText.text = indonesianTexts[random];
        }
    }
}