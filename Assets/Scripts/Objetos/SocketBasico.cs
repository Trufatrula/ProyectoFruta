using System.Collections;
using UnityEngine;

public class SocketBasico : MonoBehaviour, ISocket
{
    [SerializeField]
    public string socketID = "SocketEstandar";
    public string ItemID => socketID;
    public GameObject GameObject => gameObject;

    [SerializeField]
    private string allowedObject = "Ejemplo";
    [SerializeField]
    private Transform sitioSocket;

    private IGrabbable placedObject;

    public void Highlight()
    {

    }
    public void Unhighlight()
    {

    }
    public void Interact()
    {

    }

    public bool PlaceInSocket(IGrabbable placedObject)
    {
        if (this.placedObject == null && placedObject.ItemID.Equals(allowedObject))
        {
            this.placedObject = placedObject;
            placedObject.GameObject.transform.parent = sitioSocket;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(PlaceItemCoroutine());
            return true;
        }
        return false;
    }

    private IEnumerator PlaceItemCoroutine()
    {
        Transform objTransform = placedObject.GameObject.transform;
        while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
        {
            objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);

            yield return null;
        }
        objTransform.localPosition = Vector3.zero;
        placedObject.Drop(this);
    }

    public void TakeFromSocket()
    {
        GetComponent<Collider>().enabled = true;
        placedObject = null;
    }
}
