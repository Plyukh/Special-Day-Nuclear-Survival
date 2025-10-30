using UnityEngine;
using UnityEngine.UI;

public class Fog : MonoBehaviour
{
    [SerializeField] private Map map;

    private Animator animator;
    private Image image;
    public bool find;

    public Location location;
    public string[] randomEncounters;

    private void Awake()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    public void Find()
    {
        animator.SetTrigger("Find");
    }

    [System.Serializable]
    public class Location
    {
        public string sceneName;
        public string locationName;
        public string engLocationName;
        public string indonesianLocationName;
        public string description;
        public string engDescription;
        public string indonesianDescription;
        public Sprite spriteLocation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player Point Map")
        {
            if(randomEncounters.Length > 0)
            {
                map.encounterCell = this;

                map.timeToEncounter = Random.Range(2.5f, 5f);
                map.currentTimeToEncounter = 0;
            }
            else
            {
                map.timeToEncounter = 0;
                map.currentTimeToEncounter = 0;
            }
        }
    }
}