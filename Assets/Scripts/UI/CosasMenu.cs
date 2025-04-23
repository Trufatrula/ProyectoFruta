using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CosasMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider slider;
    [SerializeField] Animator animatorOpciones;

    [SerializeField] Image imagenIntro;
    [SerializeField] float duracionIntro;

    public bool arriba = false;

    public void SetVolumen()
    {
        float volumen = slider.value;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volumen) * 20);
    }

    public void EmpezarJuego()
    {
        StartCoroutine(CinematicaIntro(duracionIntro));
    }

    public void SetOpciones()
    {
        animatorOpciones.SetTrigger("Mover");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    private IEnumerator CinematicaIntro(float duration)
    {
        float startAlpha = imagenIntro.color.a;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 1, timeElapsed / duration);

            Color newColor = imagenIntro.color;
            newColor.a = newAlpha;
            imagenIntro.color = newColor;

            yield return null;
        }

        Color finalColor = imagenIntro.color;
        finalColor.a = 1;
        imagenIntro.color = finalColor;
    }

    public void CargarJuego()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
