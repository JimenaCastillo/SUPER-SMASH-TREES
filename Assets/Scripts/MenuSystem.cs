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
        SceneManager.LoadScene("Map");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
