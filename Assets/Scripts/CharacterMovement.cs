using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(50)]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent playerNavMeshAgent;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject effect;

    CameraZoom cameraZoom;
    Radiation radiation;
    HealthSystem healthSystem;
    CapsuleCollider capsuleCollider;

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

    int floorLayer = -1;
    int interactableLayer = -1;
    int uiLayer = -1;
    int ignoreRaycastLayer = -1;
    int invisibleToCameraLayer = -1;
    int playerLayer = -1;
    int npcLayer = -1;
    int navigationRaycastMask = ~0;

    const int RaycastBufferSize = 32;
    readonly RaycastHit[] raycastBuffer = new RaycastHit[RaycastBufferSize];

    void Awake()
    {
        floorLayer = LayerMask.NameToLayer("Floor");
        interactableLayer = LayerMask.NameToLayer("Interactable");
        uiLayer = LayerMask.NameToLayer("UI");
        ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        invisibleToCameraLayer = LayerMask.NameToLayer("Invisible to the camera");
        playerLayer = LayerMask.NameToLayer("Player");
        npcLayer = LayerMask.NameToLayer("NPC");

        navigationRaycastMask = ~(LayerMask.GetMask("UI", "Ignore Raycast"));

        if (playerCamera != null)
            cameraZoom = playerCamera.GetComponent<CameraZoom>();
        TryGetComponent(out radiation);
        TryGetComponent(out healthSystem);
        TryGetComponent(out capsuleCollider);
    }

    static bool TryFindChildInteractable(Transform targetRoot, out Interactable interactable, out Collider interactCollider)
    {
        interactable = null;
        interactCollider = null;
        for (int i = 0; i < targetRoot.childCount; i++)
        {
            Transform c = targetRoot.GetChild(i);
            if (c.TryGetComponent(out interactable))
            {
                interactCollider = c.GetComponent<Collider>();
                return true;
            }
        }

        return false;
    }

    static bool TryFindChildDoor(Transform root, out Door door)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).TryGetComponent(out door))
                return true;
        }

        door = null;
        return false;
    }

    private void LateUpdate()
    {
        if (tag != "Player")
            return;

        if (!PrimaryPointerInput.GetPrimaryUpThisFrame(out Vector2 tapScreen) || !canMove || playerCamera == null)
            return;

        if (cameraZoom != null && cameraZoom.CameraDrag)
            return;

        Ray ray = playerCamera.ScreenPointToRay(tapScreen);
        int hitCount = Physics.RaycastNonAlloc(ray, raycastBuffer, 800f, navigationRaycastMask, QueryTriggerInteraction.Collide);
        if (hitCount <= 0)
        {
            if (!Physics.Raycast(ray, out RaycastHit singleHit, 800f, navigationRaycastMask, QueryTriggerInteraction.Collide))
                return;
            raycastBuffer[0] = singleHit;
            hitCount = 1;
        }

        int best = -1;
        float bestDist = float.MaxValue;
        for (int i = 0; i < hitCount; i++)
        {
            ref RaycastHit h = ref raycastBuffer[i];
            if (h.collider == null)
                continue;
            int layer = h.collider.gameObject.layer;
            if (layer == uiLayer || layer == ignoreRaycastLayer)
                continue;

            GameObject hitGo = h.collider.gameObject;
            bool hasCubeObject = hitGo.GetComponentInChildren<CubeObject>(true) != null;

            if (layer == floorLayer || layer == interactableLayer || hasCubeObject)
            {
                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    best = i;
                }
            }
        }

        if (best < 0)
        {
            bestDist = float.MaxValue;
            for (int i = 0; i < hitCount; i++)
            {
                ref RaycastHit h = ref raycastBuffer[i];
                if (h.collider == null)
                    continue;
                int layer = h.collider.gameObject.layer;
                if (layer == uiLayer || layer == ignoreRaycastLayer)
                    continue;
                if (invisibleToCameraLayer >= 0 && layer == invisibleToCameraLayer)
                    continue;
                if (playerLayer >= 0 && layer == playerLayer)
                    continue;
                if (npcLayer >= 0 && layer == npcLayer)
                    continue;
                GameObject go = h.collider.gameObject;
                if (go.CompareTag("Player") || go.CompareTag("Sphere"))
                    continue;

                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    best = i;
                }
            }
        }

        if (best < 0)
            return;

        RaycastHit raycastHit = raycastBuffer[best];
        target = raycastHit.collider.gameObject;
        targetPoint = raycastHit.point;
        Instantiate(effect, targetPoint, effect.transform.rotation, gameObject.transform.parent);
        if (target.layer == interactableLayer)
        {
            if (TryFindChildInteractable(target.transform, out Interactable foundInteractable, out _))
            {
                playerNavMeshAgent.stoppingDistance = interatableStoppingDistance;
                interactableTarget = foundInteractable;
                interactableUI.ShowButtons(interactableTarget, character);

                return;
            }
        }
        else
        {
            CubeObject cube = target.GetComponent<CubeObject>() ?? target.GetComponentInChildren<CubeObject>(true);
            if (cube != null)
            {
                cube.ClickMesh();
                targetPoint = cube.usePosition.transform.position;
            }
            runToFloor = true;
            interactableTarget = null;
            playerNavMeshAgent.stoppingDistance = floorStoppingDistance;
        }
        if (!target.CompareTag("Player") && !target.CompareTag("Sphere") && interactableTarget == null)
        {
            interactableUI.point.SetActive(false);

            playerNavMeshAgent.SetDestination(targetPoint);
            move = true;
        }
    }

    private void FixedUpdate()
    {
        GameObject combatTarget = character.combatSystem.Target();

        if (combatTarget != null && character.stealthSystem.stealth == false)
        {
            target = combatTarget;
            playerNavMeshAgent.SetDestination(target.transform.position);
            move = true;
        }

        if(tag == "Player")
        {
            character.animator.SetBool("Stealth", character.stealthSystem.stealth);
        }
        else
        {
            if(target != null)
            {
                if (target.layer == interactableLayer)
                {
                    if (TryFindChildInteractable(target.transform, out Interactable foundInteractable, out Collider col))
                    {
                        targetPoint = col != null ? col.bounds.center : foundInteractable.transform.position;
                        interactableTarget = foundInteractable;
                    }
                }
                else
                {
                    if (combatTarget != null)
                        targetPoint = combatTarget.transform.position;
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

            combatTarget = character.combatSystem.Target();

            if (combatTarget != null && character.stealthSystem.stealth == false)
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

                if (Vector3.Distance(playerNavMeshAgent.transform.position, combatTarget.transform.position) <= Distance)
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100);
                    StopMovement();
                    character.combatSystem.StartCombat();
                    if (tag == "Player")
                    {
                        if (combatTarget.TryGetComponent(out HealthSystem targetHealth) && targetHealth.health > 0)
                            targetHealth.SpawnSlider();
                    }
                }
            }
            else
            {
                if (Vector3.Distance(playerNavMeshAgent.transform.position, targetPoint) <= playerNavMeshAgent.stoppingDistance)
                {
                    if(target != null)
                    {
                        if (target.layer == interactableLayer)
                        {
                            if (TryFindChildInteractable(target.transform, out Interactable foundInteractable, out _))
                            {
                                interactableTarget = foundInteractable;
                                StartInteract();
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
        if (other.gameObject.layer == interactableLayer)
        {
            if (TryFindChildDoor(other.gameObject.transform, out Door door))
            {
                if (door.open == false)
                {
                    selectSkill = null;
                    interactableTarget = door;
                    StartInteract();
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

                if (radiation != null)
                {
                    if (targetRoom.radiactive)
                        radiation.StartRadiation();
                    else
                        radiation.StopRadiation();
                }

                if (targetRoom.quest >= 0 && healthSystem != null)
                    healthSystem.questSystem.CompletePart(targetRoom.quest, targetRoom.questPart);

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
        if (interactableTarget.TryGetComponent<Furniture>(out _))
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
        else if (interactableTarget.TryGetComponent<Door>(out _))
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
        else if (interactableTarget.transform.parent != null && interactableTarget.transform.parent.CompareTag("Car"))
        {
            interactableTarget.InteracteblePosition(playerNavMeshAgent.gameObject);
            character.animator.SetTrigger("Use");
            character.SetWeaponTrigger(true);
        }
        else if (interactableTarget.TryGetComponent<Travel>(out _) || interactableTarget.TryGetComponent<TravelSearch>(out _))
        {
            interactableTarget.Use();
            canMove = false;
        }
        else if (interactableTarget.TryGetComponent<Person>(out Person person))
        {
            if (character.stealthSystem.stealth)
            {
                character.animator.SetTrigger("Use");
            }
            else if (person.Character.combatSystem.Aggressive == false)
            {
                person.Dialogue();
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
        if (cameraZoom != null)
            cameraZoom.OnPlayerPosition();
        else if (Camera.main != null && Camera.main.TryGetComponent<CameraZoom>(out CameraZoom mainZoom))
            mainZoom.OnPlayerPosition();
    }

    private void Interact()
    {
        if(interactableTarget != null)
        {
            if (selectSkill == null)
            {
                if (interactableTarget.TryGetComponent<Furniture>(out _) || interactableTarget.TryGetComponent<Item>(out _))
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
        Person person = interactableTarget.GetComponent<Person>();
        CombatSystem targetCombatSystem = person.Character.combatSystem;
        CharacterMovement leaderMovement = targetCombatSystem.GetComponent<CharacterMovement>();

        targetCombatSystem.CanDialogue = false;
        targetCombatSystem.Aggressive = true;
        targetCombatSystem.radius = 99999;

        foreach (var item in targetCombatSystem.Allies)
        {
            item.combatSystem.CanDialogue = false;
            item.combatSystem.Aggressive = true;
            if (leaderMovement != null && item.characterMovement != null &&
                leaderMovement.CurrentRoom != null && item.characterMovement.CurrentRoom != null &&
                leaderMovement.CurrentRoom.name == item.characterMovement.CurrentRoom.name)
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
        Room roomComp = CurrentRoom.GetComponent<Room>();
        if (blood)
        {
            footSteps.clip = audioClips[Random.Range(50, 60)];
        }
        else if(roomComp.roomType == RoomType.Metal)
        {
            footSteps.clip = audioClips[Random.Range(0,10)];
        }
        else if(roomComp.roomType == RoomType.Grass)
        {
            footSteps.clip = audioClips[Random.Range(10, 20)];
        }
        else if (roomComp.roomType == RoomType.Wood)
        {
            footSteps.clip = audioClips[Random.Range(20, 30)];
        }
        else if (roomComp.roomType == RoomType.Rock)
        {
            footSteps.clip = audioClips[Random.Range(30, 40)];
        }
        else if (roomComp.roomType == RoomType.Dirty)
        {
            footSteps.clip = audioClips[Random.Range(40, 50)];
        }

        footSteps.Play();
    }

    public void SleepCollider()
    {
        CapsuleCollider c = capsuleCollider != null ? capsuleCollider : GetComponent<CapsuleCollider>();
        if (c != null)
            c.center += new Vector3(0,-0.5f, -2);
    }
    public void BaseSleepCollider()
    {
        CapsuleCollider c = capsuleCollider != null ? capsuleCollider : GetComponent<CapsuleCollider>();
        if (c != null)
            c.center += new Vector3(0, 0.5f, 2f);
    }
    public void SitCollider()
    {
        CapsuleCollider c = capsuleCollider != null ? capsuleCollider : GetComponent<CapsuleCollider>();
        if (c != null)
            c.center += new Vector3(0, -0.25f, -0.25f);
    }
    public void BaseSitCollider()
    {
        CapsuleCollider c = capsuleCollider != null ? capsuleCollider : GetComponent<CapsuleCollider>();
        if (c != null)
            c.center += new Vector3(0, 0.25f, 0.25f);
    }
}