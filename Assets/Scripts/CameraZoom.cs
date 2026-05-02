using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-50)]
public class CameraZoom : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private InteractableUI interactableUI;

    private Vector3 touchStart;

    public Vector3 startPosition;
    public Vector3 endPosition;

    public float zoomOutMin;
    public float zoomOutMax;

    public Vector2 maxPosition;
    public Vector2 minPosition;
    public float baseX;

    public bool pointer;

    public bool CameraDrag;
    bool mouseDown;
    bool panExceededThreshold;
    Vector2 panPointerStartScreen;
    float CameraDragMinScreenPixels => Mathf.Max(32f, Mathf.Min(Screen.width, Screen.height) * 0.04f);

    private bool findPlayer;
    private Vector3 playerPosition;

    private CharacterMovement characterMovement;

    [SerializeField] private GameObject inventoryCameraPoint;
    private Vector3 baseInventoryRotation;
    private bool inventoryRotation;

    void Awake()
    {
        ResolvePlayerReferences();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ResolvePlayerReferences();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolvePlayerReferences();
    }

    void ResolvePlayerReferences()
    {
        characterMovement = null;

        if (player != null)
        {
            characterMovement = player.GetComponent<CharacterMovement>();
            if (characterMovement == null)
                characterMovement = player.GetComponentInChildren<CharacterMovement>(true);
            if (characterMovement != null)
                return;
        }

        GameObject found = GameObject.FindGameObjectWithTag("Player");
        if (found == null)
            return;

        if (player == null)
            player = found;

        characterMovement = found.GetComponent<CharacterMovement>();
        if (characterMovement == null)
            characterMovement = found.GetComponentInChildren<CharacterMovement>(true);
    }

    private void Start()
    {
        ResolvePlayerReferences();
        OnPlayerPosition();
    }

    public void OnPointerObject()
    {
        pointer = true;
    }
    public void OnExitObject()
    {
        pointer = false;
        Camera.main.transform.position = Camera.main.transform.position;
    }

    public void OnPlayerPosition()
    {
        Vector3 oldRotation = player.transform.localEulerAngles;
        Vector3 oldCameraPosition = transform.position;

        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0, 0, baseX);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        player.transform.localEulerAngles = new Vector3(10, 105, 0);

        transform.SetParent(null);
        player.transform.localEulerAngles = oldRotation;
        playerPosition = transform.position;
        transform.position = oldCameraPosition;

        OnExitObject();
        findPlayer = true;
    }

    public void InventoryCamera(bool active)
    {
        if (active)
        {
            baseInventoryRotation = player.transform.localEulerAngles;
            inventoryCameraPoint.transform.SetParent(null);
        }
        else
        {
            player.transform.localEulerAngles = baseInventoryRotation;
            inventoryCameraPoint.transform.SetParent(player.transform);
        }
    }

    public void SetInventoryRotation(bool value)
    {
        inventoryRotation = value;
    }

    private void Update()
    {
        if (inventoryRotation)
        {
            Vector3 direction = player.transform.localEulerAngles - Camera.main.ViewportToScreenPoint(Input.mousePosition);
            player.transform.localEulerAngles = new Vector3(0, direction.x * 0.00025f, 0);
        }
    }

    void LateUpdate()
    {
        if (pointer == false)
        {
            if (Input.touchCount <= 1)
            {
                if (PrimaryPointerInput.GetPrimaryDownThisFrame(out Vector2 downScreen))
                {
                    touchStart = Camera.main.ScreenToWorldPoint(downScreen);

                    startPosition = transform.position;
                    endPosition = transform.position;
                    mouseDown = true;
                    panExceededThreshold = false;
                    panPointerStartScreen = downScreen;
                    interactableUI.point.SetActive(false);
                    if (characterMovement == null)
                        ResolvePlayerReferences();
                }
                if (PrimaryPointerInput.GetPrimaryUpThisFrame(out _))
                {
                    if (mouseDown && !panExceededThreshold)
                    {
                        startPosition = endPosition = transform.position;
                    }
                    mouseDown = false;
                    panExceededThreshold = false;
                }

                if (mouseDown)
                {
                    Vector2 currentScreen = PrimaryPointerInput.GetPrimaryScreenPosition();

                    if (!panExceededThreshold)
                    {
                        if ((currentScreen - panPointerStartScreen).sqrMagnitude >= CameraDragMinScreenPixels * CameraDragMinScreenPixels)
                            panExceededThreshold = true;
                    }

                    if (panExceededThreshold &&
                        Camera.main.transform.position.z < maxPosition.x && Camera.main.transform.position.y < maxPosition.y &&
                        Camera.main.transform.position.z > minPosition.x && Camera.main.transform.position.y > minPosition.y)
                    {
                        Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(currentScreen);
                        Camera.main.transform.position += direction;
                        endPosition = transform.position;
                    }
                    else if (panExceededThreshold)
                    {
                        OnPlayerPosition();
                        mouseDown = false;
                        panExceededThreshold = false;
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }
        }

        if (startPosition != endPosition || pointer == true)
        {
            CameraDrag = true;
        }
        else
        {
            CameraDrag = false;
        }

        zoom(0);

        if (findPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, playerPosition, 3 * Time.deltaTime);
            if (Vector3.Distance(transform.position, playerPosition) <= 0.2f)
            {
                findPlayer = false;
            }
        }
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);

        float x = Camera.main.orthographicSize - 5;
    }
}