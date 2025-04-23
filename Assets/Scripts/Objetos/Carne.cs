using UnityEngine;

public class Carne : MonoBehaviour, IGrabbable
{

    [SerializeField]
    public string itemID = "CarneDeX";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    private bool isSocketed = false;

    private ISocket socket;

    private void Update()
    {
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
        GetComponent<CapsuleCollider>().isTrigger = true;
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
