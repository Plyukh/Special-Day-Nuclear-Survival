using UnityEngine;

public class MedkitItem : Item
{
    public int minHeal;
    public int maxHeal;

    public int removeRadiation;

    public AudioClip audioClip;

    public void UseMedTik(Character character)
    {
        number -= 1;

        if(removeRadiation > 0)
        {
            character.GetComponent<HealthSystem>().ApplyHealRad(removeRadiation);
        }
        else
        {
            float x = maxHeal - minHeal;
            int newMin = minHeal + Mathf.RoundToInt(x / 100 * character.FindSkill(Skills.Doctor).points);
            int heal = Random.Range(newMin, maxHeal + 1);
            character.GetComponent<HealthSystem>().ApplyHeal(heal);
        }
    }
}
