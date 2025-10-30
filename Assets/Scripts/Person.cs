using UnityEngine;
using UnityEngine.UI;

public class Person : Interactable
{
    [SerializeField] private GameObject cameraPoint;

    private Character character;
    private Dialogue dialogue;
    private DialogueSystem dialogueSystem;

    private Character player;

    public Character Character
    {
        get
        {
            return character;
        }
    }

    private void OnEnable()
    {
        dialogue = GetComponent<Dialogue>();
        character = transform.parent.GetComponent<Character>();
        dialogueSystem = character.GetComponent<HealthSystem>().questSystem.transform.parent.GetChild(9).GetComponent<DialogueSystem>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        base.OnEnable();
    }

    public override void Use()
    {
        character.Inventory.ShowInventory();
        player.Inventory.ShowInventory();

        player.Inventory.takeAllButton.SetActive(false);
    }
    public void Dialogue()
    {
        dialogueSystem.StartDialogue(cameraPoint, dialogue);
    }

    public void Barter()
    {
        player.Inventory.barter = true;
        player.Inventory.ShowInventory(true);
        character.Inventory.ShowInventory(true);
    }

    public void AddSound()
    {
        dialogueSystem.transform.parent.GetComponent<AudioSource>().Play();
    }
}