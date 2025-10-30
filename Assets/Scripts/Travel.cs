using UnityEngine;

public class Travel : Interactable
{
    [SerializeField] private Map map;

    public override void Use()
    {
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();

        map.CanTravel(true);
    }
}
