using TMPro;
using UnityEngine;

public class HPala : MonoBehaviour, IGrabbable
{

    [SerializeField]
    public string itemID = "HPala";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    private bool isSocketed = false;

    private ISocket socket;

    public bool agarrada = false;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Camera camaraReal;
    public LayerMask cavarLayer;

    public Transform targetPosition;
    public Transform originalPosition;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    private void Update()
    {
        if (!agarrada) return;
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Usar Pala");
            animator.SetTrigger("Cavar");
            Ray ray = camaraReal.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, cavarLayer))
            {
                Debug.Log("En el clavo");
                LugarDeExcavar excavaZona = hit.collider.GetComponent<LugarDeExcavar>();
                if (excavaZona != null)
                {
                    excavaZona.Desenterrar(false);
                }
            }
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
        if (socket != null)
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
        if (socket == null) return;

        this.socket = socket;

        switch (socket.ItemID)
        {
            case "SocketEstandar":
                isSocketed = true;
                break;
            default:
                break;
        }
    }
}
