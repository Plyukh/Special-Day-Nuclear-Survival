using UnityEngine;
using UnityEngine.UI;

public class StealthSystem : MonoBehaviour
{
    [SerializeField] private Character character;

    [SerializeField] private Button stealthButton;

    [SerializeField] private Image icon;
    [SerializeField] private Image sunIcon;
    [SerializeField] private Image moonIcon;

    public Image stealIcon;

    public bool stealth;
    public bool light;

    [SerializeField] private float maxRisk;
    [SerializeField] private float currentRisk;

    [HideInInspector] public bool training;

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
        //Steal Perk
        if (stealth)
        {
            light = character.characterMovement.CurrentRoom.GetComponent<Room>().light;
        }
        else if (tag == "Player" && character.PerkSystem.FindPerk(Skills.Steal, 1).Active)
        {
            light = character.characterMovement.CurrentRoom.GetComponent<Room>().light;
            if (light == false || character.PerkSystem.FindPerk(Skills.Steal, 2).Active)
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
            RectTransform iconRect = icon.GetComponent<RectTransform>();

            iconRect.position = Camera.main.WorldToScreenPoint(character.transform.position);
            iconRect.anchoredPosition = new Vector2(iconRect.anchoredPosition.x, iconRect.anchoredPosition.y + 175);
            icon.fillAmount = currentRisk / maxRisk;

            sunIcon.transform.Rotate(0, 0, 0.1f);
            moonIcon.transform.Rotate(0, 0, 0.1f);

            float baseRisk = 25;

            if (light)
            {
                if (tag == "Player" && character.PerkSystem.FindPerk(Skills.Steal, 2).Active)
                {
                    EffectsUI.SteathEffect(true, true);
                    baseRisk = 25;
                }
                else
                {
                    EffectsUI.SteathEffect(true, false);
                    baseRisk = 50;
                }

                sunIcon.gameObject.SetActive(true);
                moonIcon.gameObject.SetActive(false);
            }
            else
            {
                baseRisk = 25;
                EffectsUI.SteathEffect(true, true);

                sunIcon.gameObject.SetActive(false);
                moonIcon.gameObject.SetActive(true);
            }

            if (character.combatSystem.Target() != null)
            {
                HealthSystem healthSystemTarget = null;
                for (int i = 0; i < character.combatSystem.targets.Count; i++)
                {
                    Character characterTarget = character.combatSystem.targets[i].GetComponent<Character>();
                    healthSystemTarget = characterTarget.GetComponent<HealthSystem>();
                    healthSystemTarget.ActiveSteathOutline(true);
                    if (Vector3.Distance(transform.position, character.combatSystem.targets[i].transform.position) > characterTarget.GetComponent<CombatSystem>().radius ||
                        characterTarget.characterMovement.CurrentRoom != character.characterMovement.CurrentRoom)
                    {
                        healthSystemTarget.ActiveSteathOutline(false);
                        character.combatSystem.ResetCombat(character.combatSystem.targets[i]);
                    }
                }

                if(character.combatSystem.targets.Count > 0)
                {
                    if (character.Inventory.ShowPanel() == false)
                    {
                        currentRisk += (baseRisk - (character.FindSkill(Skills.Steal).points / 5)) * Time.deltaTime;
                    }

                    if (currentRisk >= maxRisk)
                    {
                        stealth = false;
                        icon.gameObject.SetActive(false);
                        character.Inventory.CloseInventory();

                        for (int i = 0; i < character.combatSystem.targets.Count; i++)
                        {
                            healthSystemTarget.ActiveSteathOutline(false);
                            character.combatSystem.ResetCombat(character.combatSystem.targets[i]);
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