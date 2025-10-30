using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private SkinnedMeshRenderer maleCharacter, femaleCharacter;
    [SerializeField] private Material[] skins;
    [SerializeField] private GameObject[] hair;
    [SerializeField] private GameObject[] beard;

    [SerializeField] private int currentBeard;
    [SerializeField] private int currentHair;

    private void Awake()
    {
        SelectGender(true);
        SelectSkin(0);

        character.currentHairIndex = -1;
        character.currentBeardIndex = -1;
    }

    public void SelectSkin(int index)
    {
        maleCharacter.material = skins[index];
        femaleCharacter.material = skins[index];

        character.currentSkinIndex = index;
    }
    public void SelectGender(bool male)
    {
        if (male)
        {
            maleCharacter.gameObject.SetActive(true);
            femaleCharacter.gameObject.SetActive(false);

            character.male = true;
        }
        else
        {
            maleCharacter.gameObject.SetActive(false);
            femaleCharacter.gameObject.SetActive(true);

            character.male = false;
        }
    }
    public void SelectBeard(int add)
    {
        if (currentBeard + add >= 0 && currentBeard + add < beard.Length)
        {
            for (int i = 0; i < beard.Length; i++)
            {
                beard[i].SetActive(false);
            }

            currentBeard += add;
            beard[currentBeard].SetActive(true);

            character.currentBeardIndex = currentBeard;
        }
        else
        {
            currentBeard = -1;
            for (int i = 0; i < beard.Length; i++)
            {
                beard[i].SetActive(false);
            }

            character.currentBeardIndex = -1;
        }
    }
    public void SelectHair(int add)
    {
        if (currentHair + add >= 0 && currentHair + add < hair.Length)
        {
            for (int i = 0; i < hair.Length; i++)
            {
                hair[i].SetActive(false);
            }

            currentHair += add;
            hair[currentHair].SetActive(true);

            character.currentHairIndex = currentHair;
        }
        else
        {
            currentHair = -1;
            for (int i = 0; i < hair.Length; i++)
            {
                hair[i].SetActive(false);
            }

            character.currentHairIndex = -1;
        }
    }
}
