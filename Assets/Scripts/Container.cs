using UnityEngine;

public class Container : Interactable
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private bool destroy;

    public void StartContainer()
    {
        if (destroy)
        {
            CheckDestroy();
        }
    }

    public override void Use()
    {
        Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        inventory.ShowInventory();
        player.Inventory.ShowInventory();

        if (inventory.money > 0)
        {
            inventory.AddMoney(inventory, player.Inventory, 0, true);
            player.Inventory.CheckMoney();
        }
    }

    public void OpenContainer()
    {
        if(needSkill != null)
        {
            needSkill = null;
            Destroy(GetComponent<Skill>());
            audioSource.Play();
        }
    }

    private void CheckDestroy()
    {
        foreach (var item in inventory.items)
        {
            if(item != null)
            {
                return;
            }
        }
        transform.parent.gameObject.SetActive(false);
    }
}