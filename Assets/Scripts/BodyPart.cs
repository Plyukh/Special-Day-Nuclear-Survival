using UnityEngine;
public enum Body
{
    Head,
    Torso,
    Groin,
    RightHand,
    LeftHand,
    RightLeg,
    LeftLeg
}

public class BodyPart : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    public Body body;

    public HealthSystem HealthSystem
    {
        get
        {
            return healthSystem;
        }
    }
}
