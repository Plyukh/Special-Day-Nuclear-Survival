using UnityEngine;

public class SaveToStart : MonoBehaviour
{
    [SerializeField] private SaveScript saveScript;

    public void SaveGame()
    {
        if(saveScript.GetComponent<SceneManagerScript>().sceneName != "CharecterCreator")
        {
            saveScript.SaveToStart();
        }
    }
}
