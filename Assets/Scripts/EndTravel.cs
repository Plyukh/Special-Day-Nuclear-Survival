using UnityEngine;

public class EndTravel : Interactable
{
    [SerializeField] GameObject theEndWindow;
    [SerializeField] QuestSystem questSystem;
    [SerializeField] CharacterMovement characterMovementPlayer;

    [SerializeField] int quest;
    public override void Use()
    {
        if (questSystem.quests[quest].Complete)
        {
            Camera.main.GetComponent<CameraZoom>().OnPointerObject();
            theEndWindow.SetActive(true);
        }
        else
        {
            if(languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print("Вы не закончили основное задание", Color.red);
            }
            else if(languageManager.currentLanguage == Language.English)
            {
                EventLog.Print("You haven't completed the main quest", Color.red);
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print("Kamu belum menyelesaikan tugas utama", Color.red);
            }
        }
    }
}
