using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Vector3 target;
    [HideInInspector] public GameObject alwaysTarget;
    [HideInInspector] public DamageType damageType;
    [HideInInspector] public string weaponName;

    public float damage;
    public float speed;

    [SerializeField] private GameObject destroyEffect;

    public bool rotate;

    public bool visualDestroy;

    private float destroyTime = 1f;
    private float currentTime = 0f;

    private void FixedUpdate()
    {
        if(alwaysTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, alwaysTarget.GetComponent<Collider>().bounds.center, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        if (rotate)
        {
            gameObject.transform.Rotate(1, 0, 0f);
        }

        currentTime += Time.deltaTime;
        if(currentTime >= destroyTime)
        {
            if (visualDestroy)
            {
                VisualDestroy();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BodyPart>())
        {
            BodyPart bodyPart = other.GetComponent<BodyPart>();
            bodyPart.HealthSystem.ApplyDamage(damage, bodyPart.body, damageType, weaponName);
            VisualDestroy();
        }
    }

    void VisualDestroy()
    {
        Instantiate(destroyEffect, transform.position, destroyEffect.transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
