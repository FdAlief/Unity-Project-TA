using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton agar mudah diakses dari mana saja

    [Header("Level Progress")]
    public bool[] levelCompleted; // Menyimpan status apakah level sudah selesai

    [Header("PlayerPrefs Key to Save Level Progress")]
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Agar LevelManager tetap ada di scene yang berbeda
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLevelProgress(); // Memuat progres level yang telah disimpan
    }

    // Method untuk menandai level telah selesai
    // Digunakan pada script StageManager (OnObjectiveComplete)
    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelCompleted.Length)
        {
            levelCompleted[levelIndex] = true;
            SaveLevelProgress(); // Simpan progres ke PlayerPrefs
        }
    }

    // Method untuk mengecek apakah level tertentu sudah selesai
    // Digunakan pada script LevelInput (UpdateLevelButtons)
    public bool IsLevelCompleted(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelCompleted.Length)
        {
            return levelCompleted[levelIndex];
        }
        return false;
    }

    // Simpan progres level ke PlayerPrefs
    private void SaveLevelProgress()
    {
        for (int i = 0; i < levelCompleted.Length; i++)
        {
            PlayerPrefs.SetInt(keyPrefs + i, levelCompleted[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // Load progres level dari PlayerPrefs
    private void LoadLevelProgress()
    {
        for (int i = 0; i < levelCompleted.Length; i++)
        {
            levelCompleted[i] = PlayerPrefs.GetInt(keyPrefs + i, i == 0 ? 1 : 0) == 1; // Level 1 selalu terbuka
        }
    }
}
