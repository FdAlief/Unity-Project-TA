using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Tombol back di Android
        {
            // Jika ini bukan scene pertama, kembali ke scene sebelumnya
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            else
            {
                // Jika sudah di scene pertama, bisa exit game
                Application.Quit();
            }
        }
    }

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
