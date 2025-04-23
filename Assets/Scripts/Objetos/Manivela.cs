using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Manivela : MonoBehaviour, IInteractable
{
    public string ItemID => "Manivela";
    public GameObject GameObject => gameObject;


    [SerializeField]
    private List<ObjetoMovible> objetosManivelados = new List<ObjetoMovible>();

    [SerializeField]
    private float anguloMax;
    [SerializeField]
    private float velRotacion = 5f;
    [SerializeField]
    private bool bloqueada = true;
    [SerializeField]
    private float velVolver = 5f;

    [SerializeField]
    private Transform puntoMirar;

    [SerializeField]
    private EstatuaParlanchina estatua;

    //[SerializeField]
    //private UnityEvent eventoFinal;

    private Quaternion rotacionInicial;
    private float rotacionActual;
    private bool manivelando = false;
    private bool inversidad = false;

    private bool primerEncuentro = true;

    private int dialogueIndex = 0;

    [SerializeField]
    private SocketPozo cubeta;

    [SerializeField]
    private List<GameObject> dialogos = new List<GameObject>();

    void Start()
    {
        rotacionInicial = transform.localRotation;
    }

    private void Update()
    {
        if (manivelando && !inversidad)
        {
            rotacionActual += velRotacion * Time.deltaTime;
            rotacionActual = Mathf.Clamp(rotacionActual, 0f, anguloMax);
            transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, rotacionActual);
        }
        else if (manivelando && inversidad)
        {
            rotacionActual -= velRotacion * Time.deltaTime;
            rotacionActual = Mathf.Clamp(rotacionActual, 0f, anguloMax);
            transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, rotacionActual);
        }
        else if (!bloqueada)
        {
            if (rotacionActual > 0f)
            {
                rotacionActual -= velVolver * Time.deltaTime;
                if (rotacionActual < 0f)
                    rotacionActual = 0f;
                transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, rotacionActual);
            }
        }

        MoverMovibles();

        if (manivelando && (rotacionActual == anguloMax || (rotacionActual == 0 && inversidad)))
        {
            manivelando = false;
            inversidad = !inversidad;
            Debug.Log(inversidad);

            if(inversidad)
            {
                if(primerEncuentro)
                {
                    PrimerEncuentro();
                    return;
                }
                CuboArriba();
                return;
            }
            CuboAbajo();

            //eventoFinal.Invoke();
        }
    }

    private void PrimerEncuentro()
    {
        primerEncuentro = false;
        GameManager.Instance.DialogueStarter(puntoMirar, 55, 0.25f, "POZO");
        dialogos[dialogueIndex].SetActive(true);
    }

    private void CuboArriba()
    {

    }

    private void CuboAbajo()
    {
        switch (cubeta.GetObjetoEnCubo())
        {
            case "CarneDeCiervo":
                if(dialogueIndex == 1)
                {
                    GameManager.Instance.DialogueStarter(puntoMirar, 55, 0.25f, "POZO");
                    dialogos[dialogueIndex].SetActive(true);
                    cubeta.SustituirObjeto(false);
                } else if(dialogueIndex == 2)
                {
                    GameManager.Instance.DialogueStarter(puntoMirar, 55, 0.25f, "POZO");
                    dialogos[dialogueIndex].SetActive(true);
                    cubeta.SustituirObjeto(true);
                    estatua.SaltarAlDialogo(2);
                }
                break;
            case "CarneDeOso":
                break;
            case "CarneHumana":
                break;
            default:
                break;
        }
    }

    private void MoverMovibles()
    {
        foreach(ObjetoMovible obj in objetosManivelados)
        {
            obj.Mover(rotacionActual / anguloMax);
        }
    }

    public void Interact()
    {
        manivelando = true;
    }

    public void Highlight()
    {
        if (manivelando && Input.GetKeyUp(KeyCode.E)) { manivelando = false; };
    }

    public void Unhighlight()
    {
        manivelando = false;
    }

    public void AvanzarDialogo()
    {
        dialogueIndex++;
        TerminarDialogo();
    }
    public void TerminarDialogo()
    {
        GameManager.Instance.DialogueEnder();
    }
}
