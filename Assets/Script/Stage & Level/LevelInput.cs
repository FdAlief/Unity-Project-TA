using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInput : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button[] levelButtons; // Semua button level

    private void Start()
    {
        UpdateLevelButtons();
    }

    // Perbarui status interactable tombol berdasarkan progres level di LevelManager
    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (LevelManager.Instance.IsLevelCompleted(i))
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }
}
