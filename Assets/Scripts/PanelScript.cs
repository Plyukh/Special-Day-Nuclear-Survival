using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private GameObject[] windows;

    public void SelectWindow(int index)
    {
        for (int i = 0; i < windows.Length; i++)
        {
            if(i == index)
            {
                windows[i].SetActive(true);
                buttonImages[i].color = Color.white;
            }
            else
            {
                windows[i].SetActive(false);
                buttonImages[i].color = Color.gray;
            }
        }
    }
}
