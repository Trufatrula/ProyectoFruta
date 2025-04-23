using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketPozo : MonoBehaviour, ISocket
{
    [SerializeField]
    public string socketID = "SocketPozo";
    public string ItemID => socketID;
    public GameObject GameObject => gameObject;

    [SerializeField]
    private List<string> allowedObjects = new List<string>();
    [SerializeField]
    private List<GameObject> obsequios = new List<GameObject>();


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
        if (this.placedObject == null && allowedObjects.Contains(placedObject.ItemID))
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

    public string GetObjetoEnCubo()
    {
        if(placedObject != null)
        {
            return placedObject.ItemID;
        }
        return "Nada";
    }

    public void SustituirObjeto(bool brujula)
    {
        placedObject.GameObject.SetActive(false);

        if(!brujula)
        {
            TakeFromSocket();
            return;
        }
        placedObject = obsequios[0].GetComponent<IGrabbable>();
        placedObject.GameObject.SetActive(true);
        placedObject.Drop(this);
    }
}
