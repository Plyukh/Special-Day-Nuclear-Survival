using UnityEngine;

public class TravelSearch : Interactable
{
    [SerializeField] private SceneManagerScript sceneManager;
    public string sceneName;
    public override void Use()
    {
        sceneManager.GetComponent<SaveScript>().Save();
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();
        sceneManager.LoadScene(sceneName);
    }
}
