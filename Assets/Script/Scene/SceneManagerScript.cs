using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    // Method Pindah Scene
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    // Method Quit
    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
