using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LugarDeExcavar : MonoBehaviour
{
    private int contadorDesenterrar;
    [SerializeField] private GameObject trufaPrefab;
    [SerializeField] private ParticleSystem particulasTruferas;
    private GameObject trufaInstance;
    private Vector3 aumentoPosicion;
    private float tramosAumento;

    [SerializeField]
    private bool oculto = true;

    private bool tieneBejelito = true;

    private BoxCollider colliderCavar;
    private AudioSource audioSource;

    void Start()
    {
        colliderCavar = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        SpawnearBejelito();
    }


    private void SpawnearBejelito()
    {
        contadorDesenterrar = Random.Range(4, 9);

        float randomY = Random.Range(-0.5f, -0.3f);

        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, transform.position.z);
        trufaInstance = Instantiate(trufaPrefab, spawnPosition, Quaternion.identity, transform);

        aumentoPosicion = trufaInstance.transform.position;
        tramosAumento = Mathf.Abs(trufaInstance.transform.localPosition.y / contadorDesenterrar);
        Debug.Log($"Tramos Aumento: {tramosAumento}, Contador Desenterrar: {contadorDesenterrar}");
    }

    public void Desenterrar(bool todo)
    {
        Debug.Log("DESENTERRANDO BEJELITO");
        //particulasTruferas.Stop();
        if (!todo)
        {
            if(oculto)
            {
                particulasTruferas.Play();
            }
            aumentoPosicion.y += tramosAumento;
            trufaInstance.transform.position = aumentoPosicion;
            contadorDesenterrar--;
        }
        else
        {
            aumentoPosicion.y += tramosAumento * contadorDesenterrar;
            trufaInstance.transform.position = aumentoPosicion;
            contadorDesenterrar = 0;
        }

        if (contadorDesenterrar == 0)
        {
            var rb = trufaInstance.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            trufaInstance.transform.parent = null;
            trufaInstance.layer = 7;
            trufaInstance.GetComponent<Bejelito>().ActivarBejelito(true);
            colliderCavar.enabled = false;
            tieneBejelito = false;
            audioSource.Play();

            //FindAnyObjectByType<HBrujula>().CambiarTarget();
            particulasTruferas.Stop();
            
            StartCoroutine(RespawnDeBejelito());
        }
    }

    private IEnumerator RespawnDeBejelito()
    {
        yield return new WaitForSeconds(15f);
        SpawnearBejelito();
        colliderCavar.enabled = true;
        tieneBejelito = true;
        particulasTruferas.Play();
    }

    public bool GetTieneBejelito()
    {
        return tieneBejelito;
    }
}