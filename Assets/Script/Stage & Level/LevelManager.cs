using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton agar mudah diakses dari mana saja

    [Header("Level Progress")]
    public bool[] levelUnlocked; // Menyimpan status apakah level sudah selesai
    public bool[] lastCompletedLevel; // status Level yang sudah selesai

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
        if (levelIndex >= 0 && levelIndex < levelUnlocked.Length)
        {
            levelUnlocked[levelIndex] = true;
            lastCompletedLevel[levelIndex] = true; // Sekarang, completed array juga diaktifkan
            SaveLevelProgress();
        }
    }

    // Method untuk mengecek apakah level tertentu sudah selesai
    // Digunakan pada script LevelInput (UpdateLevelButtons)
    public bool IsLevelCompleted(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelUnlocked.Length)
        {
            return levelUnlocked[levelIndex];
        }
        return false;
    }

    // Simpan progres level ke PlayerPrefs
    // Digunakan pada Method CompleteLevel & Script StageManager (OnObjectiveComplete)
    public void SaveLevelProgress()
    {
        for (int i = 0; i < levelUnlocked.Length; i++)
        {
            PlayerPrefs.SetInt(keyPrefs + "_unlocked_" + i, levelUnlocked[i] ? 1 : 0);
            PlayerPrefs.SetInt(keyPrefs + "_completed_" + i, lastCompletedLevel[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // Load progres level dari PlayerPrefs
    // Digunakan pada Method Awake
    private void LoadLevelProgress()
    {
        for (int i = 0; i < levelUnlocked.Length; i++)
        {
            // Default: Level 0 selalu terbuka, lainnya tertutup
            int defaultUnlock = (i == 0) ? 1 : 0;
            levelUnlocked[i] = PlayerPrefs.GetInt(keyPrefs + "_unlocked_" + i, defaultUnlock) == 1;

            // Default completed: semua false
            lastCompletedLevel[i] = PlayerPrefs.GetInt(keyPrefs + "_completed_" + i, 0) == 1;
        }
    }

    // Method Reload Save Data untuk ketika Restart
    // Digunakan pada Script MainMenuManager (OnClickRestart)
    public void ReloadLevelProgress()
    {
        LoadLevelProgress(); // Panggil ulang dari PlayerPrefs
    }
}
