using UnityEngine;
using UnityEngine.Events;

public class Botoncin : MonoBehaviour, IInteractable
{
    public UnityEvent evento;

    public string ItemID => "Boton";
    public bool IsGrabbable => false;
    public GameObject GameObject => gameObject;

    public void Highlight()
    {
        Debug.Log("JILIGHT");
    }

    public void Unhighlight()
    {

    }

    public void Interact()
    {
        evento.Invoke();
    }
}
