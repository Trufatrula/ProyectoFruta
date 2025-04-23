using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HBrujula : MonoBehaviour, IGrabbable
{

    [SerializeField]
    public string itemID = "HBrujula";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    [SerializeField]
    private GameObject aguja;

    [SerializeField]
    private List<LugarDeExcavar> listaExcavaciones = new List<LugarDeExcavar>();

    [SerializeField]
    private Transform target;
    private LugarDeExcavar excavacionElegida;

    public float agujaRotationSpeed = 5f;

    public float jitterMagnitude = 2f;

    private float jitterTimer = 0f;

    [SerializeField]
    private float brujulaThreshold = 0.25f;

    private bool isSocketed = false;

    private ISocket socket;

    public bool agarrada = false;

    public Transform targetPosition;
    public Transform originalPosition; 
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;
    private bool isRightClickHeld = false;

    private void Start()
    {
        CambiarTarget();
    }

    public void CambiarTarget()
    {
        foreach (LugarDeExcavar excavacion in listaExcavaciones)
        {
            if (excavacion.GetTieneBejelito())
            {
                target = excavacion.gameObject.transform;
                excavacionElegida = target.GetComponent<LugarDeExcavar>();
                break;
            }
        }
    }

    private void Update()
    {
        if(!agarrada) return;

        if(aguja != null && isRightClickHeld && Vector3.Distance(targetPosition.localPosition, transform.localPosition) <= brujulaThreshold)
        {
            Vector3 direction = target.position - aguja.transform.position;
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

            aguja.transform.rotation = Quaternion.Slerp(aguja.transform.rotation, targetRotation, Time.deltaTime * agujaRotationSpeed);
        }

        if (Input.GetMouseButton(1))
        {
            isRightClickHeld = true;
            if(!excavacionElegida.GetTieneBejelito())
            {
                CambiarTarget();
            }
        }
        else
        {
            isRightClickHeld = false;
        }

        if (isRightClickHeld)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition.localPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRotation, rotationSpeed * 30f * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPosition, moveSpeed * 10f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, rotationSpeed * Time.deltaTime);
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
