using UnityEngine;

public enum Skills
{
    Guns,
    Unarmed,
    MeleeWeapons,
    Steal,
    Lockpick,
    Repair,
    Science,
    Doctor,
    Speech,
    Barter
}

public class Skill : MonoBehaviour
{
    public Sprite sprite;
    public Skills skill;
    public int attributePoints;
    public int lvlPoints;
    public int points;

    [HideInInspector] public int newPoints;

    public string ruName;
    public string engName;
    public string indonesianName;

    public string ruDescription;
    public string engDescription;
    public string indonesianDescription;

    private void Update()
    {
        points = attributePoints + lvlPoints + newPoints;
    }
}
