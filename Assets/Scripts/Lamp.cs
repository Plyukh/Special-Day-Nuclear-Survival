using UnityEngine;

public class Lamp : MonoBehaviour
{
    private Light light;

    private float radius;

    private Character player;

    private void Start()
    {
        light = GetComponent<Light>();
    }

    private void Update()
    {
        radius = light.range;

        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > radius)
            {
                player.stealthSystem.light = false;
                player = null;
            }
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward);
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
            {
                player = hit.collider.GetComponent<Character>();
                player.stealthSystem.light = true;
            }
        }
    }
}
