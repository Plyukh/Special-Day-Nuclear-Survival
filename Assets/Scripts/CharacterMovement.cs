using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent playerNavMeshAgent;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject effect;

    [SerializeField] private Character character;
    [SerializeField] private InteractableUI interactableUI;
    [SerializeField] private Skill selectSkill;

    [SerializeField] private GameObject room;
    [SerializeField] private GameObject target;
    public Interactable interactableTarget;
    public Interactable oldInteractableTarget;
    private Vector3 targetPoint;

    private bool move;
    public bool canMove = true;

    private float floorStoppingDistance = 0.25f;
    private float interatableStoppingDistance = 1f;

    public float idleSpeed;
    public float crouchSpeed;
    public float walkSpeed;
    public float runSpeed;

    public bool interact;

    [SerializeField] private AudioSource footSteps;
    [SerializeField] private AudioClip[] audioClips;

    private bool blood;

    public GameObject CurrentRoom
    {
        get
        {
            return room;
        }
        set
        {
            room = value;
        }
    }

    public GameObject Target
    {
        get
        {
            return target;
        }
    }

    public Vector3 lastPosition;

    private float stuckTimer;
    private float stuckThreshold = 0.5f;
    private float positionTolerance = 0.01f;
    private bool runToFloor;

    private void FixedUpdate()
    {
        if(character.combatSystem.Target() != null && character.stealthSystem.stealth == false)
        {
            target = character.combatSystem.Target();
            playerNavMeshAgent.SetDestination(target.transform.position);
            move = true;
        }

        if(tag == "Player")
        {
            RaycastHit raycastHit;
            if (Input.GetMouseButtonUp(0) && Input.touchCount == 1 && canMove && playerCamera.GetComponent<CameraZoom>().CameraDrag == false)
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit))
                {
                    target = raycastHit.collider.gameObject;
                    targetPoint = raycastHit.point;
                    Instantiate(effect,targetPoint, effect.transform.rotation, gameObject.transform.parent);
                    if (target.layer == LayerMask.NameToLayer("Interactable"))
                    {
                        for (int i = 0; i < target.transform.childCount; i++)
                        {
                            if (target.transform.GetChild(i).GetComponent<Interactable>())
                            {
                                playerNavMeshAgent.stoppingDistance = interatableStoppingDistance;
                                interactableTarget = target.transform.GetChild(i).GetComponent<Interactable>();
                                interactableUI.ShowButtons(interactableTarget, character);

                                return;
                            }
                        }
                    }
                    else
                    {
                        if (target.GetComponent<CubeObject>())
                        {
                            target.GetComponent<CubeObject>().ClickMesh();
                            targetPoint = target.GetComponent<CubeObject>().usePosition.transform.position;
                        }
                        runToFloor = true;
                        interactableTarget = null;
                        playerNavMeshAgent.stoppingDistance = floorStoppingDistance;
                    }
                    if (target.tag != "Player" && target.tag != "Sphere" && interactableTarget == null)
                    {
                        interactableUI.point.SetActive(false);

                        playerNavMeshAgent.SetDestination(targetPoint);
                        move = true;
                    }
                }
            }
            character.animator.SetBool("Stealth", character.stealthSystem.stealth);
        }
        else
        {
            if(target != null)
            {
                if (target.layer == LayerMask.NameToLayer("Interactable"))
                {
                    for (int i = 0; i < target.transform.childCount; i++)
                    {
                        if (target.transform.GetChild(i).GetComponent<Interactable>())
                        {
                            targetPoint = target.transform.GetChild(i).GetComponent<Collider>().bounds.center;
                            interactableTarget = target.transform.GetChild(i).GetComponent<Interactable>();

                            break;
                        }
                    }
                }
                else
                {
                    targetPoint = character.combatSystem.Target().transform.position;
                }
                playerNavMeshAgent.SetDestination(targetPoint);
                move = true;
            }
        }

        if (move)
        {
            if (character.Inventory.Encumbered())
            {
                character.animator.SetBool("Walk", true);
            }
            else
            {
                character.animator.SetBool("Running", true);
            }

            if (character.combatSystem.Target() != null && character.stealthSystem.stealth == false)
            {
                float Distance = 0;
                if (character.currentWeapon != null)
                {
                    Distance = character.currentWeapon.distance;

                    //Guns Perk
                    if (tag == "Player")
                    {
                        if (character.PerkSystem.FindPerk(Skills.Guns, 1).Active)
                        {
                            if (character.currentWeapon.weaponType != WeaponType.OneHandMeleeWeapon && character.currentWeapon.weaponType != WeaponType.OneHandMeleeWeapon)
                            {
                                Distance *= 2f;
                            }
                        }
                    }
                }
                else
                {
                    Distance = 1.25f;
                }

                if (Vector3.Distance(playerNavMeshAgent.transform.position, character.combatSystem.Target().transform.position) <= Distance)
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100);
                    StopMovement();
                    character.combatSystem.StartCombat();
                    if (tag == "Player")
                    {
                        if (character.combatSystem.Target().GetComponent<HealthSystem>().health > 0)
                        {
                            character.combatSystem.Target().GetComponent<HealthSystem>().SpawnSlider();
                        }
                    }
                }
            }
            else
            {
                if (Vector3.Distance(playerNavMeshAgent.transform.position, targetPoint) <= playerNavMeshAgent.stoppingDistance)
                {
                    if(target != null)
                    {
                        if (target.layer == LayerMask.NameToLayer("Interactable"))
                        {
                            for (int i = 0; i < target.transform.childCount; i++)
                            {
                                if (target.transform.GetChild(i).GetComponent<Interactable>())
                                {
                                    interactableTarget = target.transform.GetChild(i).GetComponent<Interactable>();
                                    StartInteract();

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        CanMove();
                    }

                    StopMovement();
                }
            }

            if (tag == "Player")
            {
                if (Vector3.Distance(transform.position, lastPosition) < positionTolerance)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= stuckThreshold && runToFloor)
                    {
                        stuckTimer = 0;
                        character.animator.SetBool("Running", false);
                        character.animator.SetBool("Walk", false);
                        playerNavMeshAgent.ResetPath();
                        playerNavMeshAgent.velocity = new Vector3(0, 0, 0);

                        target = null;
                        move = false;
                        runToFloor = false;
                    }
                }
                else
                {
                    stuckTimer = 0;
                }
                lastPosition = transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            for (int i = 0; i < other.gameObject.transform.childCount; i++)
            {
                if (other.gameObject.transform.GetChild(i).GetComponent<Door>())
                {
                    Door door = other.gameObject.transform.GetChild(i).GetComponent<Door>();

                    if (door.open == false)
                    {
                        selectSkill = null;
                        interactableTarget = door;
                        StartInteract();
                    }
                    break;
                }
            }
        }
        if (other.gameObject.tag == "Car")
        {
            Container car = other.gameObject.transform.GetChild(1).GetComponent<Container>();
            selectSkill = null;
            interactableTarget = car;
            StartInteract();
        }
        if (other.gameObject.tag == "Room Point")
        {
            room = other.transform.parent.parent.gameObject;

            if(tag == "Player")
            {
                Room targetRoom = room.GetComponent<Room>();

                if (targetRoom.radiactive)
                {
                    GetComponent<Radiation>().StartRadiation();
                }
                else
                {
                    GetComponent<Radiation>().StopRadiation();
                }

                if (targetRoom.quest >= 0)
                {
                    GetComponent<HealthSystem>().questSystem.CompletePart(room.GetComponent<Room>().quest, targetRoom.questPart);
                }

                if(targetRoom.find == false)
                {
                    ExperienceSystem.AddXP(25);
                    targetRoom.find = true;
                    targetRoom.MeshDisabled();
                }
            }
        }
        if(other.gameObject.tag == "Blood")
        {
            blood = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Blood")
        {
            blood = false;
        }
    }

    public void StartInteract()
    {
        if (interactableTarget.GetComponent<Furniture>())
        {
            if (selectSkill == null)
            {
                interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
                Interact();
                character.SetWeaponTrigger(true);
            }
            else
            {
                interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject, true);
                character.animator.SetTrigger("Use");
                character.SetWeaponTrigger(true);
            }
        }
        else if (interactableTarget.GetComponent<Door>())
        {
            if (interactableTarget.training)
            {
                canMove = true;
                interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
            }
            else
            {
                interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
                character.animator.SetTrigger("Use");
                character.SetWeaponTrigger(true);
            }
        }
        else if (interactableTarget.transform.parent.tag == "Car")
        {
            interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
            character.animator.SetTrigger("Use");
            character.SetWeaponTrigger(true);
        }
        else if (interactableTarget.GetComponent<Travel>() || interactableTarget.GetComponent<TravelSearch>())
        {
            interactableTarget.Use();
            canMove = false;
        }
        else if (interactableTarget.GetComponent<Person>())
        {
            if (character.stealthSystem.stealth)
            {
                character.animator.SetTrigger("Use");
            }
            else if (interactableTarget.GetComponent<Person>().Character.combatSystem.Aggressive == false)
            {
                interactableTarget.GetComponent<Person>().Dialogue();
            }
        }
        else
        {
            if (interactableTarget.training)
            {
                //interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
                canMove = true;
            }
            else
            {
                //interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
                character.animator.SetTrigger("Use");
                character.SetWeaponTrigger(true);
            }
        }

        oldInteractableTarget = interactableTarget;
        interact = true;
        StopMovement();
        Camera.main.GetComponent<CameraZoom>().OnPlayerPosition();
    }

    private void Interact()
    {
        if(interactableTarget != null)
        {
            if (selectSkill == null)
            {
                if (interactableTarget.GetComponent<Furniture>() || interactableTarget.GetComponent<Item>())
                {
                    interactableTarget.Use(character.animator);
                }
                else
                {
                    interactableTarget.Use();
                }
            }
            else
            {
                interactableTarget.UseSkill(selectSkill);
            }

            interactableTarget.clickCollider.enabled = false;
            selectSkill = null;
        }
        else
        {
            CanMove();
        }
    }

    public void StopMovement()
    {
        character.animator.SetBool("Running", false);
        character.animator.SetBool("Walk", false);
        playerNavMeshAgent.ResetPath();
        playerNavMeshAgent.velocity = new Vector3(0, 0, 0);

        target = null;
        move = false;
    }

    public void CanMove()
    {
        canMove = true;
    }
    public void MoveToInteractable()
    {
        canMove = false;
        move = true;
        targetPoint = interactableTarget.GetComponent<Collider>().bounds.center;
        playerNavMeshAgent.SetDestination(targetPoint);
    }
    public void MoveToCombat()
    {
        CombatSystem targetCombatSystem = interactableTarget.GetComponent<Person>().Character.combatSystem;

        targetCombatSystem.CanDialogue = false;
        targetCombatSystem.Aggressive = true;
        targetCombatSystem.radius = 99999;

        foreach (var item in targetCombatSystem.Allies)
        {
            item.combatSystem.CanDialogue = false;
            item.combatSystem.Aggressive = true;
            if(targetCombatSystem.GetComponent<CharacterMovement>().CurrentRoom.name == item.characterMovement.CurrentRoom.name)
            {
                item.combatSystem.radius = 99999;
            }
        }

        canMove = false;
        move = true;
        targetPoint = interactableTarget.GetComponent<Collider>().bounds.center;
        playerNavMeshAgent.SetDestination(targetPoint);
    }

    public void DisabledNavMeshAgent()
    {
        playerNavMeshAgent.enabled = false;
    }

    private void IdleSpeed()
    {
        playerNavMeshAgent.speed = idleSpeed;
    }
    private void WalkSpeed()
    {
        playerNavMeshAgent.speed = walkSpeed;
    }
    private void RunSpeed()
    {
        playerNavMeshAgent.speed = runSpeed;
    }

    private void CrouchSpeed()
    {
        playerNavMeshAgent.speed = crouchSpeed;
    }

    private void EnabledInteractableTarget()
    {
        interact = false;
        oldInteractableTarget.clickCollider.enabled = true;
        oldInteractableTarget = null;
    }

    public void SelectSkill(int Index)
    {
        selectSkill = character.CharacterSkills[Index];
    }

    public void PlayFootstepsSound()
    {
        Room room = CurrentRoom.GetComponent<Room>();
        if (blood)
        {
            footSteps.clip = audioClips[Random.Range(50, 60)];
        }
        else if(room.roomType == RoomType.Metal)
        {
            footSteps.clip = audioClips[Random.Range(0,10)];
        }
        else if(room.roomType == RoomType.Grass)
        {
            footSteps.clip = audioClips[Random.Range(10, 20)];
        }
        else if (room.roomType == RoomType.Wood)
        {
            footSteps.clip = audioClips[Random.Range(20, 30)];
        }
        else if (room.roomType == RoomType.Rock)
        {
            footSteps.clip = audioClips[Random.Range(30, 40)];
        }
        else if (room.roomType == RoomType.Dirty)
        {
            footSteps.clip = audioClips[Random.Range(40, 50)];
        }

        footSteps.Play();
    }

    public void SleepCollider()
    {
        GetComponent<CapsuleCollider>().center += new Vector3(0,-0.5f, -2);
    }
    public void BaseSleepCollider()
    {
        GetComponent<CapsuleCollider>().center += new Vector3(0, 0.5f, 2f);
    }
    public void SitCollider()
    {
        GetComponent<CapsuleCollider>().center += new Vector3(0, -0.25f, -0.25f);
    }
    public void BaseSitCollider()
    {
        GetComponent<CapsuleCollider>().center += new Vector3(0, 0.25f, 0.25f);
    }
}