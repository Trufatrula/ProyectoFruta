using System.Collections.Generic;
using UnityEngine;

public class PuertaFinal : MonoBehaviour
{
    [SerializeField]
    private List<BejelitoComprobar> bejelitos = new List<BejelitoComprobar>();

    public void ComprobarBejelitos()
    {
        foreach(BejelitoComprobar bejelito in bejelitos)
        {
            if (!bejelito.GetConteniendo()) return;
        }
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Abrir");
    }
}
