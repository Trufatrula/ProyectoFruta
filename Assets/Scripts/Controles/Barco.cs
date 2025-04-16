using UnityEngine;

public class Barco : MonoBehaviour, IInteractable
{
    [SerializeField]
    public string itemID = "Barco";
    public string ItemID => itemID;

    public GameObject GameObject => gameObject;

    [SerializeField]
    private GameObject zonaMirar;

    [SerializeField]
    private float velMaxima = 10f;
    [SerializeField]
    private float aceleracion = 5f;
    [SerializeField]
    private float deceleracion = 5f;

    [SerializeField]
    private float velGirar = 2f;
    [SerializeField]
    private float smoothingTimeRotacion = 0.2f;

    [SerializeField]
    private float amplitudOleaje = 0.5f;
    [SerializeField]
    private float frecuenciaOleaje = 0.5f;

    private float currentSpeed = 0f;
    private float speedSmoothVelocity = 0f;
    private float currentTurnInput = 0f;
    private float targetTurnInput = 0f;

    private float originalY;

    void Start()
    {
        originalY = transform.position.y;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        float targetSpeed = moveInput * velMaxima;


        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity,
                                        moveInput != 0 ? 1f / aceleracion : 1f / deceleracion);


        targetTurnInput = turnInput * velGirar;

        currentTurnInput = Mathf.Lerp(currentTurnInput, targetTurnInput, Time.deltaTime / smoothingTimeRotacion);

        transform.Rotate(Vector3.up, currentTurnInput);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

        Vector3 pos = transform.position;
        pos.y = originalY + Mathf.Sin(Time.time * frecuenciaOleaje) * amplitudOleaje;
        transform.position = pos;
    }

    public void Highlight()
    {

    }

    public void Unhighlight()
    {

    }

    public void Interact()
    {
        Movement player = FindAnyObjectByType<Movement>();

        CameraController camera = FindAnyObjectByType<CameraController>();
        camera.LockCamera(true);

        Destroy(player);

        camera.transform.parent = zonaMirar.transform;
        camera.transform.localPosition = Vector3.zero;

    }
}