using UnityEngine;

public class Attributes : MonoBehaviour
{
    public int Strength;
    public int Agility;
    public int Intelligence;
    public int Charisma;

    public int points;

    private void AddAttribute(ref int attribute, int value)
    {
        if(value == 1)
        {
            if(points > 0)
            {
                if (attribute >= 1 && attribute <= 9)
                {
                    attribute += value;
                    points -= value;
                }
            }
        }
        else
        {
            if (attribute >= 2 && attribute <= 10)
            {
                attribute += value;
                points -= value;
            }
        }
    }

    public void AddStrength(int value)
    {
        AddAttribute(ref Strength, value);
    }
    public void AddAgility(int value)
    {
        AddAttribute(ref Agility, value);
    }
    public void AddIntelligence(int value)
    {
        AddAttribute(ref Intelligence, value);
    }
    public void AddCharisma(int value)
    {
        AddAttribute(ref Charisma, value);
    }

    public void AddPoints()
    {
        points += 10 + Intelligence;
    }

    public void ResetPoints(bool Base = true)
    {
        if (Base)
        {
            Strength = 1;
            Agility = 1;
            Intelligence = 1;
            Charisma = 1;

            points = 21;
        }
        else
        {
            Character character = GetComponent<Character>();

            for (int i = 0; i < character.CharacterSkills.Length; i++)
            {
                points += character.CharacterSkills[i].newPoints;
                character.CharacterSkills[i].newPoints = 0;
            }
        }
    }
}
