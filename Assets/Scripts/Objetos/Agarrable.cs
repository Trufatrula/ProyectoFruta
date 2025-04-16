using UnityEngine;

public class Agarrable : MonoBehaviour, IGrabbable
{

    [SerializeField]
    public string itemID = "Agarrable";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    private bool isSocketed = false;

    private ISocket socket;

    public Transform target;

    public float rotationSpeed = 5f;

    public float jitterMagnitude = 2f;

    private float jitterTimer = 0f;

    private void Update()
    {
        if(isSocketed)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // 0 en la y para que se quede fijo.
            if (direction == Vector3.zero)
                return; // Por si acaso está en la misma posición. (Hacer que gire descontroladamente.)

            // Rotacion necesaria para apuntar a la direccion.
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Actualizar el movimiento de incertidumbre.
            jitterTimer += Time.deltaTime;
            float jitter = Mathf.Sin(jitterTimer * 2f) * jitterMagnitude * 0.5f
                           + Random.Range(-jitterMagnitude, jitterMagnitude) * 0.1f;

            // Agregar el jitter a la rotacion en y.
            Vector3 targetEuler = targetRotation.eulerAngles;
            targetEuler.y += jitter;
            targetRotation = Quaternion.Euler(targetEuler);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void Highlight()
    {

    }
    public void Unhighlight()
    {

    }
    public void Interact()
    {

    }

    public void Grab()
    {
        Debug.Log("RAAA");
        if(socket != null)
        {
            isSocketed = false;
            socket.TakeFromSocket();
            socket = null;
        }

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public void Drop(ISocket socket)
    {
        gameObject.layer = 7;
        if(socket == null) return;

        this.socket = socket;

        switch(socket.ItemID)
        {
            case "SocketEstandar":
                isSocketed = true;
                break;
            default:
                break;
        }
    }
}
