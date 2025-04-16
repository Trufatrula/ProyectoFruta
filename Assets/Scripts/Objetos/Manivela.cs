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
    private UnityEvent eventoFinal;

    private Quaternion rotacionInicial;
    private float rotacionActual;
    private bool manivelando = false;
    private bool inversidad = false;

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

        if (rotacionActual == anguloMax || (rotacionActual == 0 && inversidad))
        {
            manivelando = false;
            inversidad = !inversidad;
            eventoFinal.Invoke();
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
}
