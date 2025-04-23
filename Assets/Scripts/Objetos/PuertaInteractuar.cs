using UnityEngine;
using UnityEngine.Rendering;

public class PuertaInteractuar : MonoBehaviour, IInteractable
{
    [SerializeField]
    public string itemID = "Puerta";
    public string ItemID => itemID;
    public bool IsGrabbable => true;
    public GameObject GameObject => gameObject;

    private Animator animator;
    private AudioSource audioPuerta;
    private BoxCollider boxCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioPuerta = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
    }
    public void Highlight()
    {

    }
    public void Unhighlight()
    {

    }
    public void Interact()
    {
        animator.SetTrigger("Abrir");
        audioPuerta.Play();
        boxCollider.enabled = false;
    }
}
