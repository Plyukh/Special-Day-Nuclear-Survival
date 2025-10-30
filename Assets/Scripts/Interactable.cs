using UnityEngine;

public class Interactable : MonoBehaviour, IUse
{
    public Skill needSkill;
    public Attributes needAttributes;

    public GameObject rightUsePosition;
    public GameObject leftUsePosition;
    public GameObject usePosition;

    public bool training;

    [HideInInspector] public Collider clickCollider;
    private Room room;
    protected Outline outline;
    protected AudioSource audioSource;
    protected LanguageManager languageManager;

    public Outline Outline
    {
        get
        {
            return outline;
        }
        set
        {
            outline = value;
        }
    }

    protected void OnEnable()
    {
        clickCollider = transform.parent.GetComponent<Collider>();
        if (GetComponent<Outline>())
        {
            if(transform.parent.name != "Car")
            {
                room = transform.parent.parent.GetComponent<Room>();
            }
            outline = GetComponent<Outline>();
        }
        if (GetComponent<AudioSource>())
        {
            audioSource = GetComponent<AudioSource>();
        }
        if(languageManager == null)
        {
            languageManager = FindFirstObjectByType<LanguageManager>();
        }
    }

    protected void Update()
    {
        if (outline != null)
        {
            if (transform.parent.name == "Car")
            {
                outline.OutlineWidth = 2;
            }
            else if (room.find)
            {
                outline.OutlineWidth = 2;
            }
            else
            {
                outline.OutlineWidth = 0;
            }
        }
    }

    public virtual void Use()
    {

    }
    public virtual void Use(Animator CharacterAnimator)
    {

    }

    public virtual void UseSkill(Skill CurrentSkill)
    {
        if(needSkill != null)
        {
            if (CurrentSkill.skill == needSkill.skill)
            {
                int points = CurrentSkill.points;

                //Lockpick Perk
                if (CurrentSkill.skill == Skills.Lockpick && CurrentSkill.transform.parent.parent.GetComponent<Character>().PerkSystem.FindPerk(CurrentSkill.skill, 0).Active)
                {
                    if (GetComponent<Door>())
                    {
                        points += 25;
                    }
                }
                //Repair Perk
                if (CurrentSkill.skill == Skills.Repair && CurrentSkill.transform.parent.parent.GetComponent<Character>().PerkSystem.FindPerk(CurrentSkill.skill, 0).Active)
                {
                    if (GetComponent<Container>())
                    {
                        points += 25;
                    }
                }

                if (points >= needSkill.points)
                {
                    if(languageManager.currentLanguage == Language.Russian)
                    {
                        EventLog.Print("Применение навыка \"" + CurrentSkill.ruName + "\" прошел успешно", Color.green);
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        EventLog.Print("The skill \"" + CurrentSkill.engName + "\" was successfully applied", Color.green);
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        EventLog.Print("Keterampilan \"" + CurrentSkill.indonesianName + "\" berhasil diterapkan", Color.green);
                    }

                    if (needSkill.points <= 25)
                    {
                        ExperienceSystem.AddXP(25);
                    }
                    else if (needSkill.points > 25 && needSkill.points <= 50)
                    {
                        ExperienceSystem.AddXP(50);
                    }
                    else if (needSkill.points > 50 && needSkill.points <= 75)
                    {
                        ExperienceSystem.AddXP(75);
                    }
                    else if (needSkill.points > 75)
                    {
                        ExperienceSystem.AddXP(100);
                    }

                    if(needSkill.points == 100)
                    {
                        if(needSkill.skill == Skills.Lockpick)
                        {
                            //FindFirstObjectByType<GooglePlayAchievements>().UnlockAchievement(13);
                        }
                    }

                    if (GetComponent<Door>())
                    {
                        GetComponent<Door>().OpenDoor();
                        GetComponent<Door>().Use();
                    }
                    else
                    {
                        needSkill = null;
                        Destroy(GetComponent<Skill>());
                        audioSource.Play();
                    }
                }
                else
                {
                    if (languageManager.currentLanguage == Language.Russian)
                    {
                        EventLog.Print("Недостаточно навыка", Color.red);
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        EventLog.Print("Not enough skill", Color.red);
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        EventLog.Print("Keterampilan tidak cukup", Color.red);
                    }
                }
            }
            else
            {
                if (languageManager.currentLanguage == Language.Russian)
                {
                    EventLog.Print("Ничего не происходит", Color.red);
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    EventLog.Print("Nothing happens", Color.red);
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    EventLog.Print("Tidak terjadi apa-apa", Color.red);
                }
            }
        }
        else
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print("Ничего не происходит", Color.red);
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                EventLog.Print("Nothing happens", Color.red);
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print("Tidak terjadi apa-apa", Color.red);
            }
        }
    }

    public void InteracteblePosition(GameObject player, bool UsePosition = false)
    {
        if(UsePosition == false)
        {
            if (player.transform.position.z > transform.position.z)
            {
                player.transform.position = new Vector3(leftUsePosition.transform.position.x, player.transform.position.y, leftUsePosition.transform.position.z);
                player.transform.rotation = leftUsePosition.transform.rotation;
            }
            else
            {
                player.transform.position = new Vector3(rightUsePosition.transform.position.x, player.transform.position.y, rightUsePosition.transform.position.z);
                player.transform.rotation = rightUsePosition.transform.rotation;
            }
        }
        else
        {
            player.transform.position = new Vector3(usePosition.transform.position.x, player.transform.position.y, usePosition.transform.position.z);
            player.transform.rotation = usePosition.transform.rotation;
        }
    }

    public bool CheckAttribute(Attributes Attributes)
    {
        if(Attributes.Strength < needAttributes.Strength)
        {
            return false;
        }
        if (Attributes.Agility < needAttributes.Agility)
        {
            return false;
        }
        if (Attributes.Intelligence < needAttributes.Intelligence)
        {
            return false;
        }
        if (Attributes.Charisma < needAttributes.Charisma)
        {
            return false;
        }
        return true;
    }
}
