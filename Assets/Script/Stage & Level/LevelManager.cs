using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Unlocking System")]
    [SerializeField] private bool[] levelUnlocked; // Menyimpan status apakah level tertentu sudah terbuka

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Agar LevelManager tidak hilang saat pergantian scene
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLevelStatus(); // Load status level yang telah tersimpan
    }

    // Mengecek apakah level tertentu sudah terbuka
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelUnlocked.Length)
        {
            return levelUnlocked[levelIndex];
        }
        return false;
    }

    // Membuka level berikutnya jika level sebelumnya selesai
    public void UnlockLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelUnlocked.Length)
        {
            levelUnlocked[levelIndex] = true;
            SaveLevelStatus();
        }
    }

    // Simpan status level ke PlayerPrefs
    private void SaveLevelStatus()
    {
        for (int i = 0; i < levelUnlocked.Length; i++)
        {
            PlayerPrefs.SetInt("LevelUnlocked_" + i, levelUnlocked[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // Load status level dari PlayerPrefs
    private void LoadLevelStatus()
    {
        for (int i = 0; i < levelUnlocked.Length; i++)
        {
            levelUnlocked[i] = PlayerPrefs.GetInt("LevelUnlocked_" + i, i == 0 ? 1 : 0) == 1;
        }
    }
}
