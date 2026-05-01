using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    // XY = 0 перед показом и при закрытии; Z = 1 для UI; аниматор доводит до 1.
    static readonly Vector3 InteractableButtonHiddenScale = new Vector3(0f, 0f, 1f);

    [SerializeField] private Button[] interactableButtons;
    public GameObject point;
    public GameObject cancel;

    Interactable lastShownInteractable;
    int lastShownFrame = -1;

    public void ShowButtons(Interactable InteractableObject, Character Player)
    {
        // Один вызов на кадр для одного объекта (на случай двойного срабатывания ввода)
        if (InteractableObject == lastShownInteractable && lastShownFrame == Time.frameCount)
            return;
        lastShownInteractable = InteractableObject;
        lastShownFrame = Time.frameCount;

        Camera.main.GetComponent<CameraZoom>().OnPointerObject();

        // Сначала скрываем корень, чтобы не проигрывалась анимация поверх старых кнопок, затем один раз показываем готовое окно
        point.SetActive(false);
        cancel.SetActive(false);
        CloseButtons();

        if (InteractableObject.needSkill != null)
        {
            for (int i = 0; i < interactableButtons.Length; i++)
            {
                if (InteractableObject.needSkill.skill.ToString() == interactableButtons[i].name)
                {
                    var go = interactableButtons[i].gameObject;
                    go.transform.localScale = InteractableButtonHiddenScale;
                    go.SetActive(true);
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
                interactableButtons[6].transform.localScale = InteractableButtonHiddenScale;
                interactableButtons[6].gameObject.SetActive(true);
                interactableButtons[7].transform.localScale = InteractableButtonHiddenScale;
                interactableButtons[7].gameObject.SetActive(true);
            }
            else
            {
                interactableButtons[5].transform.localScale = InteractableButtonHiddenScale;
                interactableButtons[5].gameObject.SetActive(true);
                interactableButtons[6].transform.localScale = InteractableButtonHiddenScale;
                interactableButtons[6].gameObject.SetActive(true);
            }
        }
        else if (InteractableObject.GetComponent<Travel>())
        {
            interactableButtons[8].transform.localScale = InteractableButtonHiddenScale;
            interactableButtons[8].gameObject.SetActive(true);
        }
        else if (InteractableObject.GetComponent<TravelSearch>())
        {
            interactableButtons[9].transform.localScale = InteractableButtonHiddenScale;
            interactableButtons[9].gameObject.SetActive(true);
        }
        else
        {
            interactableButtons[0].transform.localScale = InteractableButtonHiddenScale;
            interactableButtons[0].gameObject.SetActive(true);
        }

        point.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(InteractableObject.transform.position);
        cancel.SetActive(true);
        point.SetActive(true);
    }

    public void CloseButtons()
    {
        for (int i = 0; i < interactableButtons.Length; i++)
        {
            var t = interactableButtons[i].transform;
            t.localScale = InteractableButtonHiddenScale;
            interactableButtons[i].gameObject.SetActive(false);
        }
    }
}