using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool isActiveInThisScene = true;
    public float delayTime = 0.5f;

    [Header("Tutorial Panels (Multiple)")]
    // Array untuk menyimpan semua panel tutorial yang akan ditampilkan
    public GameObject[] tutorialPanels;

    [Header("Tutorial Keys (One per panel)")]
    // Array string untuk menyimpan key PlayerPrefs masing-masing panel
    // Digunakan untuk melacak apakah panel tersebut sudah pernah ditampilkan
    public string[] tutorialKeys;

    void Start()
    {
        if (!isActiveInThisScene) return;

        // Jika key belum ada di PlayerPrefs (artinya belum pernah tampil)
        if (!PlayerPrefs.HasKey(tutorialKeys[0]))
        {
            // Tampilkan panel tutorial dengan sedikit delay (1 detik)
            StartCoroutine(DelayPanelTutorial(0));
        }
    }

    // Fungsi publik untuk menampilkan panel tutorial secara manual lewat index
    // Misalnya dipanggil saat player menang, atau tekan tombol tertentu
    public void ShowTutorial(int index)
    {
        // Cegah index yang invalid
        if (index < 0 || index >= tutorialPanels.Length) return;

        // Aktifkan panel sesuai index
        tutorialPanels[index].SetActive(true);

        // Simpan bahwa tutorial ini sudah pernah ditampilkan
        PlayerPrefs.SetInt(tutorialKeys[index], 1);
        PlayerPrefs.Save();

        Debug.Log("Tutorial sudah tampil");
    }

    // Coroutine untuk memberikan delay sebelum menampilkan tutorial
    IEnumerator DelayPanelTutorial(int index)
    {
        yield return new WaitForSeconds(delayTime); // Delay 1 detik
        ShowTutorial(index); // Panggil fungsi show
    }

    // Fungsi untuk menutup panel tutorial berdasarkan index
    public void CloseTutorial(int index)
    {
        // Cegah index invalid
        if (index < 0 || index >= tutorialPanels.Length) return;

        // Nonaktifkan panel
        tutorialPanels[index].SetActive(false);
    }

    // Method untuk menampilkan Panel Tutorial
    // Digunakan pada Button dan Script lain untuk Trigger
    public void TutorialTrigger(int index)
    {
        if (!isActiveInThisScene) return;

        // Jika key belum ada di PlayerPrefs (artinya belum pernah tampil)
        if (!PlayerPrefs.HasKey(tutorialKeys[index]))
        {
            // Tampilkan panel tutorial dengan sedikit delay (1 detik)
            StartCoroutine(DelayPanelTutorial(index));
        }
    }
}
