using System.Collections.Generic;
using UnityEngine;

public class EstatuaParlanchina : MonoBehaviour, IHablable
{
    public string ItemID => "Estatua";
    public GameObject GameObject => gameObject;

    private int dialogueIndex = 0;

    [SerializeField]
    private List<GameObject> dialogos = new List<GameObject>();

    [SerializeField]
    private Transform cabeza;

    [SerializeField]
    private float fovHablar = 100f;

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
        if (item != null)
        {
            switch (item.ItemID)
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
            GameManager.Instance.DialogueStarter(cabeza, fovHablar, 1, "Agonía");
            dialogos[dialogueIndex].SetActive(true);
        }
    }

    public int GetDialogoIndex()
    {
        return dialogueIndex;
    }

    public void AvanzarDialogoIndex(bool finalizar)
    {
        dialogueIndex++;
        if(finalizar) TerminarDialogo();
    }

    public void TerminarDialogo()
    {
        GameManager.Instance.DialogueEnder();
    }

    public void SaltarAlDialogo(int dialogo)
    {
        dialogueIndex = dialogo;
    }

}
