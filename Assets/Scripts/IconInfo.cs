using UnityEngine;
using UnityEngine.EventSystems;

public class IconInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite icon;
    public string nameIcon;
    public string engNameIcon;
    public string indonesianNameIcon;
    public string descriptionIcon;
    public string engDescriptionIcon;
    public string indonesianDescriptionIcon;

    public InfoPanel infoPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.ShowInfo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
