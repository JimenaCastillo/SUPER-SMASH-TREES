using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    // Rango de scenes
    public int escenaMin = 1;
    public int escenaMax = 3;

    public void Jugar()
    {
        int escenaAleatoria = UnityEngine.Random.Range(escenaMin, escenaMax + 1);
        SceneManager.LoadScene(escenaAleatoria);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
