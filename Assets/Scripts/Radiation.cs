using UnityEngine;
using UnityEngine.UI;

public class Radiation : MonoBehaviour
{
    [SerializeField] private Animator radiationIcon;
    [SerializeField] private Slider radiationSlider;

    Character character;
    HealthSystem healthSystem;
    CharacterMovement characterMovement;

    private float maxRad = 1000;
    public float currentRad;
    public float rad;

    void Awake()
    {
        TryGetComponent(out character);
        TryGetComponent(out healthSystem);
        TryGetComponent(out characterMovement);
    }

    public void StartRad()
    {
        radiationSlider.maxValue = maxRad;

        if (currentRad > 0)
        {
            radiationIcon.SetBool("Rad", true);
            if (characterMovement != null && characterMovement.CurrentRoom != null &&
                characterMovement.CurrentRoom.GetComponent<Room>().radiactive)
            {
                StartRadiation();
            }
            else
            {
                StopRadiation();
            }
        }
    }

    private void Update()
    {
        float newRad = 0;

        if (rad > 0)
        {
            EffectsUI.RadiationEffect(true);

            if (character != null && healthSystem != null && character.currentArmor != null)
            {
                newRad = rad / 100 * character.currentArmor.protection[4] + healthSystem.protection[4];
            }
            else if(healthSystem != null && healthSystem.protection[4] > 0)
            {
                newRad = rad / 100 * healthSystem.protection[4];
            }
        }
        else
        {
            EffectsUI.RadiationEffect(false);
        }

        currentRad += (rad - newRad) * Time.deltaTime;
        radiationSlider.value = currentRad;

        if (currentRad >= maxRad)
        {
            currentRad = maxRad - 1;
            rad = 0;
            radiationIcon.SetBool("Rad", false);
            if (healthSystem != null)
                healthSystem.Death();
        }
        if (currentRad <= 0)
        {
            currentRad = 0;
            radiationIcon.SetBool("Rad",false);
        }
    }

    public void StartRadiation()
    {
        rad = 20;
        radiationIcon.SetBool("Rad", true);
    }
    public void StopRadiation()
    {
        rad = -10;
    }
}