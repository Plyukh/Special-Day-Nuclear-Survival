using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    [SerializeField] private Button[] interactableButtons;
    public GameObject point;
    public GameObject cancel;

    public void ShowButtons(Interactable InteractableObject, Character Player)
    {
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();

        point.SetActive(true);
        cancel.SetActive(true);
        point.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(InteractableObject.transform.position);

        CloseButtons();

        if (InteractableObject.needSkill != null)
        {
            for (int i = 0; i < interactableButtons.Length; i++)
            {
                if (InteractableObject.needSkill.skill.ToString() == interactableButtons[i].name)
                {
                    interactableButtons[i].gameObject.SetActive(true);
                    Text skillText = interactableButtons[i].transform.GetChild(1).GetComponent<Text>();

                    int points = Player.FindSkill(InteractableObject.needSkill.skill).points;

                    //Lockpick Perk
                    if (InteractableObject.needSkill.skill == Skills.Lockpick && Player.PerkSystem.FindPerk(Skills.Lockpick, 0).Active)
                    {
                        if (InteractableObject.GetComponent<Door>())
                        {
                            points += 25;
                        }
                    }
                    //Repair Perk
                    if (InteractableObject.needSkill.skill == Skills.Repair && Player.PerkSystem.FindPerk(Skills.Repair, 0).Active)
                    {
                        if (InteractableObject.GetComponent<Container>())
                        {
                            points += 25;
                        }
                    }

                    skillText.text = points + "/" + InteractableObject.needSkill.points;

                    if (points >= InteractableObject.needSkill.points)
                    {
                        skillText.color = new Color32(0,255,0,255);
                    }
                    else
                    {
                        skillText.color = new Color32(255, 0, 0, 255);
                    }
                }
            }
        }
        else if (InteractableObject.GetComponent<Person>())
        {
            if (Player.stealthSystem.stealth)
            {
                interactableButtons[6].gameObject.SetActive(true);
                interactableButtons[7].gameObject.SetActive(true);
            }
            else
            {
                interactableButtons[5].gameObject.SetActive(true);
                interactableButtons[6].gameObject.SetActive(true);
            }
        }
        else if (InteractableObject.GetComponent<Travel>())
        {
            interactableButtons[8].gameObject.SetActive(true);
        }
        else if (InteractableObject.GetComponent<TravelSearch>())
        {
            interactableButtons[9].gameObject.SetActive(true);
        }
        else
        {
            interactableButtons[0].gameObject.SetActive(true);
        }
    }

    public void CloseButtons()
    {
        for (int i = 0; i < interactableButtons.Length; i++)
        {
            interactableButtons[i].gameObject.SetActive(false);
        }
    }
}