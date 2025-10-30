using UnityEngine;

public class Door : Interactable
{
    private Animator animator;
    public Room nextRoom;
    public bool open;

    private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClips;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        base.OnEnable();
    }

    public override void Use()
    {
        if (open)
        {
            animator.SetTrigger("Close");
        }
        else
        {
            if (needSkill == null)
            {
                animator.SetTrigger("Open");
            }
            else
            {
                if (languageManager.currentLanguage == Language.Russian)
                {
                    EventLog.Print("Заперто", Color.red);
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    EventLog.Print("Locked", Color.red);
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    EventLog.Print("Terkunci", Color.red);
                }
            }
        }
    }

    public void OpenDoor()
    {
        if(needSkill != null)
        {
            needSkill = null;

            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
    }

    private void Open()
    {
        open = true;

        if (nextRoom != null)
        {
            if(nextRoom.find == false)
            {
                nextRoom.find = true;
                ExperienceSystem.AddXP(25);
                nextRoom.MeshDisabled();
            }
        }

        outline.OutlineMode = Outline.Mode.OutlineVisible;

        audioSource.clip = audioClips[1];
        audioSource.Play();
    }
    private void Close()
    {
        open = false;
        outline.OutlineMode = Outline.Mode.OutlineAll;

        audioSource.clip = audioClips[2];
        audioSource.Play();
    }
}
