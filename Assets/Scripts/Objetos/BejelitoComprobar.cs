using JetBrains.Annotations;
using UnityEngine;

public class BejelitoComprobar : MonoBehaviour
{
    [SerializeField]
    private bool contenedorAzul = true;

    [SerializeField]
    private PuertaFinal puerta;

    private bool conteniendo = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Bejelito>() != null)
        {
            if(contenedorAzul)
            {
                if (other.GetComponent<Bejelito>().ItemID.Equals("BejelitoAzul"))
                {
                    conteniendo = true;
                    other.GetComponent<ConexionBejelito>().enabled = true;
                    puerta.ComprobarBejelitos();
                }
            }
            else
            {
                if (other.GetComponent<Bejelito>().ItemID.Equals("BejelitoRojo"))
                {
                    conteniendo = true;
                    other.GetComponent<ConexionBejelito>().enabled = true;
                    puerta.ComprobarBejelitos();
                }
            }
        }
    }

    public bool GetConteniendo()
    {
        return conteniendo;
    }
}
