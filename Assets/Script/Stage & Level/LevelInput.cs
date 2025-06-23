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
    [SerializeField] private Sprite completedSprite; // Sprite disable untuk button yang sudah completed
    [SerializeField] private Sprite defaultSprite; // Sprite disable untuk button yang default

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


            // Level hanya bisa dimainkan jika sudah terbuka dan belum selesai
            bool shouldBeInteractable = isUnlocked && !isCompleted;

            Button btn = levelButtons[i]; // Referensi tombol
            btn.interactable = shouldBeInteractable; // Atur apakah tombol bisa diklik atau tidak

            // Tampilkan indikator "Completed" jika level sudah selesai
            if (completedInfo.Length > i && completedInfo[i] != null)
            {
                completedInfo[i].SetActive(isCompleted);
            }

            // Ambil spriteState saat ini
            SpriteState spriteState = btn.spriteState;

            // Atur disabledSprite berdasarkan kondisi
            if (!shouldBeInteractable)
            {
                if (isCompleted && completedSprite != null)
                {
                    // Jika level sudah completed → gunakan sprite completed
                    spriteState.disabledSprite = completedSprite;
                }
                else if (defaultSprite != null)
                {
                    // Jika belum completed → gunakan sprite default
                    spriteState.disabledSprite = defaultSprite;
                }

                // Terapkan kembali spriteState ke button
                btn.spriteState = spriteState;
            }
        }
    }
}
