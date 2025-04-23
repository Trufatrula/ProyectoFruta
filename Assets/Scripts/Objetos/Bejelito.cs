using UnityEngine;

public class Bejelito : MonoBehaviour, IGrabbable
{

    [SerializeField]
    public string itemID = "Bejelito";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    private bool isSocketed = false;

    private ISocket socket;

    private bool bejelitoActivado = false;

    public float hoverHeight = 10f;
    public float springConstant = 20f;
    public float dampingConstant = 5f;

    private Rigidbody rb;
    private SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!bejelitoActivado) return;

        // Altura del hover
        float heightError = hoverHeight - transform.position.y;

        // Calcular fuerza hacia arriba, como si fuera un muelle
        float upwardForce = springConstant * heightError - dampingConstant * rb.linearVelocity.y;
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
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
        if (socket != null)
        {
            isSocketed = false;
            socket.TakeFromSocket();
            socket = null;
        }

        rb.isKinematic = true;
        sphereCollider.isTrigger = true;
    }

    public void Drop(ISocket socket)
    {
        gameObject.layer = 7;
        if (socket == null)
        {
            rb.isKinematic = false;
            sphereCollider.isTrigger = false;
            bejelitoActivado = true;

            return;
        }
        //this.socket = socket;

        switch (socket.ItemID)
        {
            case "SocketEstandar":
                //isSocketed = true;
                break;
            default:
                break;
        }
    }

    public void ActivarBejelito(bool activar)
    {
        bejelitoActivado = activar;
    }
}