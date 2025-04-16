using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class VigilanteDelPozo : MonoBehaviour, IHablable
{
    public string ItemID => "Vigilante";
    public GameObject GameObject => gameObject;

    private int dialogueIndex = 0;

    [SerializeField]
    private List<GameObject> dialogos = new List<GameObject>();

    [SerializeField]
    private Transform cabeza;

    [SerializeField]
    private float fovHablar = 50f;

    private void Start()
    {

    }

    public void Highlight()
    {
        Debug.Log("JILIGHT");
    }

    public void Unhighlight()
    {

    }

    public void Interact()
    {

    }

    public void Hablar(IGrabbable item)
    {
        if(item != null)
        {
            switch(item.ItemID)
            {
                case "Bejelito":
                    break;
                case "Brujula":
                    break;
                default:
                    break;
            }
        }
        else
        {
            GameManager.Instance.DialogueStarter(cabeza, fovHablar, 1, "Vigilante");
            dialogos[dialogueIndex].SetActive(true);
        }
    }

    public int GetDialogoIndex()
    {
        return dialogueIndex;
    }

    public void AvanzarDialogoIndex()
    {
        dialogueIndex++;
        TerminarDialogo();
    }

    public void TerminarDialogo()
    {
        GameManager.Instance.DialogueEnder();
    }

}
