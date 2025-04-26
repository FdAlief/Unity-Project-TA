using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInput : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button[] levelButtons; // Semua button level

    [Header("UI Indicator")]
    public GameObject[] completedInfo; // Sama jumlahnya dengan levelButtons

    private void Start()
    {
        UpdateLevelButtons();
    }

    // Perbarui status interactable tombol berdasarkan progres level di LevelManager
    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = LevelManager.Instance.IsLevelCompleted(i); // Gunakan isUnlocked untuk button
            levelButtons[i].interactable = isUnlocked;

            // Cek apakah level sudah selesai, untuk UI indikator
            bool isCompleted = LevelManager.Instance.lastCompletedLevel[i];
            completedInfo[i].SetActive(isCompleted);
        }
    }
}
