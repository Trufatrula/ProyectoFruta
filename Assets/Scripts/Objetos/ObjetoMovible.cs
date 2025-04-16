using UnityEngine;

public class ObjetoMovible : MonoBehaviour
{
    private Vector3 puntoInicio;

    [SerializeField]
    private Transform puntoFinal;

    private Transform posicionActual;

    private void Start()
    {
        puntoInicio = transform.position;
        posicionActual = transform;
    }
    
    public void Mover(float indice)
    {
        transform.position = Vector3.Lerp(puntoInicio, puntoFinal.position, indice);
    }
}   
