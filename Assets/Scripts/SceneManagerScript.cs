using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerScript : MonoBehaviour
{
    public string sceneName;
    [SerializeField] private GameObject loadingWindow;

    public Vector3 playerPosition;
    public GameObject currentRoom;

    public Room[] rooms;
    public bool[] find;
    public Character[] characters;
    public float[] health;
    public Mesh[] deathbody;
    public bool[] deathHair; 
    public bool[] aggressive;
    public bool[] canDialogue;
    public bool[] hasLeft;
    public Dialogue[] dialogues;
    public int[] currentNodes;
    public RepairObject[] repairObjects;
    public string[] lastRepairObject;
    public PowerBoxes[] powerBoxes;
    public bool[] works;
    public Interactable[] interactableObjects;
    public bool[] openDoors;
    public int[] skills;
    public Inventory[] inventories;
    public int[] money;
    public int[] itemsID;
    public float[] itemsNumber;
    public bool[] itemsEquipped;
    public bool[] itemsSell;

    public bool car;
    public bool reset;

    public void LoadScene(string SceneName)
    {
        loadingWindow.SetActive(true);
        StartCoroutine(Loading(SceneName));
    }

    IEnumerator Loading(string SceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}