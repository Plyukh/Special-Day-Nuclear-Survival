using UnityEngine;
using UnityEngine.UI;

public class Perk : MonoBehaviour
{
    public bool Active;

    public string perkName;
    public string engPerkName;
    public string indonesianPerkName;
    public string perkDescription;
    public string engPerkDescription;
    public string indonesianPerkDescription;
    public int id;

    public Sprite perkSprite;

    [SerializeField] private Image icon;
    [SerializeField] private AudioSource audioSource;

    public Skills skill;
    public int needPoints;

    public void UpdatePerk(Skills skill, float value)
    {
        if(skill == this.skill)
        {
            if (value == needPoints)
            {
                if (gameObject.activeInHierarchy)
                {
                    GetComponent<Animator>().SetTrigger("Perk");
                }
                if (Active)
                {
                    SelectColor(Color.yellow);
                }
                else
                {
                    SelectColor(Color.white);
                    if (gameObject.activeInHierarchy)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }
    public void UpdatePerk()
    {
        GetComponent<Animator>().SetTrigger("Perk");
        if (Active)
        {
            SelectColor(Color.yellow);
        }
        else
        {
            SelectColor(Color.white);
        }
    }

    public void SelectColor(Color color)
    {
        icon.color = color;
    }
}
