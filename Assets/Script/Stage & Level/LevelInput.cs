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
            bool isCompleted = LevelManager.Instance.lastCompletedLevel[i]; // Sudah complete?
            bool isUnlocked = LevelManager.Instance.IsLevelCompleted(i); // Dianggap "unlocked"?

            // Tombol hanya aktif kalau level unlocked dan belum completed
            levelButtons[i].interactable = isUnlocked && !isCompleted;

            // Tampilkan indikator "Completed" jika level sudah selesai
            completedInfo[i].SetActive(isCompleted);
        }
    }
}
