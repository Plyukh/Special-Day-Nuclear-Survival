using UnityEngine;

enum FurnitureType
{
    Bed,
    Chair
}

public class Furniture : Interactable
{
    [SerializeField] FurnitureType furnitureType;

    public override void Use(Animator CharacterAnimator)
    {
        if (furnitureType == FurnitureType.Bed)
        {
            CharacterAnimator.SetTrigger("Sleep");
        }
        else if (furnitureType == FurnitureType.Chair)
        {
            CharacterAnimator.SetTrigger("Sit");
        }
    }
}
