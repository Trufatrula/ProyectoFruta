using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDelJuego : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Personaje")
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}
