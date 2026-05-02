using UnityEngine;
using UnityEngine.UI;

public class StealthSystem : MonoBehaviour
{
    [SerializeField] private Character character;

    [SerializeField] private Button stealthButton;

    [SerializeField] private Image icon;
    [SerializeField] private Image sunIcon;
    [SerializeField] private Image moonIcon;

    RectTransform iconRectTransform;
    Camera mainCamera;

    GameObject cachedRoomObject;
    Room cachedRoom;

    Perk perkStealSlot1;
    Perk perkStealSlot2;

    public Image stealIcon;

    public bool stealth;
    public bool light;

    [SerializeField] private float maxRisk;
    [SerializeField] private float currentRisk;

    [HideInInspector] public bool training;

    void Awake()
    {
        if (icon != null)
            iconRectTransform = icon.GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        RefreshPerkCache();
    }

    void RefreshPerkCache()
    {
        perkStealSlot1 = null;
        perkStealSlot2 = null;
        if (character == null || character.PerkSystem == null)
            return;
        perkStealSlot1 = character.PerkSystem.FindPerk(Skills.Steal, 1);
        perkStealSlot2 = character.PerkSystem.FindPerk(Skills.Steal, 2);
    }

    void RefreshRoomCache()
    {
        GameObject roomGo = character != null && character.characterMovement != null
            ? character.characterMovement.CurrentRoom
            : null;

        if (roomGo == null)
        {
            cachedRoom = null;
            cachedRoomObject = null;
            return;
        }

        if (roomGo == cachedRoomObject && cachedRoom != null)
            return;

        cachedRoomObject = roomGo;
        cachedRoom = roomGo.GetComponent<Room>();
    }

    bool StealPerk1Active => perkStealSlot1 != null && perkStealSlot1.Active;
    bool StealPerk2Active => perkStealSlot2 != null && perkStealSlot2.Active;

    public void Stealth()
    {
        if (stealth)
        {
            stealth = false;
            currentRisk = 0;

            icon.gameObject.SetActive(false);
            EffectsUI.SteathEffect(false);
        }
        else
        {
            stealth = true;
            character.SetWeaponTrigger(true);

            icon.gameObject.SetActive(true);
            RefreshPerkCache();

            if (stealthButton != null)
            {
                stealthButton.interactable = false;
            }
        }
    }

    public void BotchedSteal()
    {
        if (stealth)
        {
            currentRisk = maxRisk;
        }
    }

    private void Update()
    {
        RefreshRoomCache();

        //Steal Perk
        if (stealth)
        {
            light = cachedRoom != null && cachedRoom.light;
        }
        else if (tag == "Player" && StealPerk1Active)
        {
            light = cachedRoom != null && cachedRoom.light;
            if (light == false || StealPerk2Active)
            {
                EffectsUI.AccuracySteathEffect(true);
            }
            else
            {
                EffectsUI.AccuracySteathEffect(false);
            }
        }

        if (stealth)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (iconRectTransform != null && mainCamera != null)
            {
                iconRectTransform.position = mainCamera.WorldToScreenPoint(character.transform.position);
                iconRectTransform.anchoredPosition = new Vector2(iconRectTransform.anchoredPosition.x, iconRectTransform.anchoredPosition.y + 175);
            }

            if (icon != null)
                icon.fillAmount = currentRisk / maxRisk;

            if (sunIcon != null)
                sunIcon.transform.Rotate(0, 0, 0.1f);
            if (moonIcon != null)
                moonIcon.transform.Rotate(0, 0, 0.1f);

            float baseRisk = 25;

            if (light)
            {
                if (tag == "Player" && StealPerk2Active)
                {
                    EffectsUI.SteathEffect(true, true);
                    baseRisk = 25;
                }
                else
                {
                    EffectsUI.SteathEffect(true, false);
                    baseRisk = 50;
                }

                if (sunIcon != null)
                    sunIcon.gameObject.SetActive(true);
                if (moonIcon != null)
                    moonIcon.gameObject.SetActive(false);
            }
            else
            {
                baseRisk = 25;
                EffectsUI.SteathEffect(true, true);

                if (sunIcon != null)
                    sunIcon.gameObject.SetActive(false);
                if (moonIcon != null)
                    moonIcon.gameObject.SetActive(true);
            }

            GameObject combatTargetGo = character.combatSystem.Target();

            if (combatTargetGo != null)
            {
                HealthSystem healthSystemTarget = null;
                var targets = character.combatSystem.targets;
                for (int i = 0; i < targets.Count; i++)
                {
                    GameObject tg = targets[i];
                    if (tg == null)
                        continue;
                    Character characterTarget = tg.GetComponent<Character>();
                    healthSystemTarget = tg.GetComponent<HealthSystem>();
                    CombatSystem combatSys = tg.GetComponent<CombatSystem>();
                    healthSystemTarget.ActiveSteathOutline(true);
                    if (Vector3.Distance(transform.position, tg.transform.position) > combatSys.radius ||
                        characterTarget.characterMovement.CurrentRoom != character.characterMovement.CurrentRoom)
                    {
                        healthSystemTarget.ActiveSteathOutline(false);
                        character.combatSystem.ResetCombat(tg);
                    }
                }

                if(targets.Count > 0)
                {
                    if (character.Inventory.ShowPanel() == false)
                    {
                        currentRisk += (baseRisk - (character.FindSkill(Skills.Steal).points / 5)) * Time.deltaTime;
                    }

                    if (currentRisk >= maxRisk)
                    {
                        stealth = false;
                        if (icon != null)
                            icon.gameObject.SetActive(false);
                        character.Inventory.CloseInventory();

                        for (int i = 0; i < targets.Count; i++)
                        {
                            healthSystemTarget.ActiveSteathOutline(false);
                            character.combatSystem.ResetCombat(targets[i]);
                        }
                    }
                }
                else
                {
                    if (currentRisk > 0)
                    {
                        currentRisk -= (baseRisk - (character.FindSkill(Skills.Steal).points / 25)) * Time.deltaTime;
                    }
                    else
                    {
                        currentRisk = 0;
                    }
                }
            }
            else
            {
                if(currentRisk > 0)
                {
                    currentRisk -= (baseRisk - (character.FindSkill(Skills.Steal).points / 25)) * Time.deltaTime;
                }
                else
                {
                    currentRisk = 0;
                }
            }
        }
    }

    public void InteractableButton()
    {
        if(stealthButton != null)
        {
            if (training == false)
            {
                stealthButton.interactable = true;
            }
        }
    }
    public void NotInteractableButton()
    {
        if (stealthButton != null)
        {
            if (training == false)
            {
                stealthButton.interactable = false;
            }
        }
    }
}