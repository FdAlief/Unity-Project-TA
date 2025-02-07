using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInput : MonoBehaviour
{
    [Header("Script DIsbale & Enable")]
    public MonoBehaviour[] scriptEnable; // Untuk mengaktifkan kembali sistem Raycast

    [Header("Stage Buttons")]
    [SerializeField] private Button[] stageButtons; // Array Button untuk setiap stage

    [Header("PlayerPrefs Key to Unlock Stage")]
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;

    private void Start()
    {
        // Nonaktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = false; // Nonaktifkan script
            }
        }

        // Setiap button berdasarkan status yang disimpan
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] != null)
            {
                if (i == 0)
                {
                    // Pastikan stage index 0 selalu aktif
                    stageButtons[i].interactable = true;
                    SaveStageUnlock(0); // Simpan ke PlayerPrefs agar tetap terbuka
                }
                else
                {
                    // Muat / load dari PlayerPrefs
                    bool isUnlocked = LoadStageUnlock(i);
                    stageButtons[i].interactable = isUnlocked;
                }
            }
        }
    }

    // Method untuk memilih Objective yang dijalankan
    // Digunakan pada UI Button Stage Input Menu
    public void ChooseStage(int index)
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }

        if (stageManager != null)
        {
            stageManager.SetTargetScoreIndex(index);
        }
        else
        {
            Debug.LogError("StageManager masih null saat memilih stage!");
        }
    }

    // Method ini untuk mengaktifkan Button Stage selanjutnya
    // Digunakan dan dipanggil oleh StageManager (OnObjectiveComplete) ketika stage objective selesai
    public void UnlockNextStage(int stageIndex)
    {
        if (stageIndex < stageButtons.Length)
        {
            stageButtons[stageIndex].interactable = true;
        }
    }

    // Method untuk menyimpan status unlock stage playerprefs
    // Digunakan dan dipanggil oleh StageManager (OnObjectiveComplete) ketika stage objective selesai
    public void SaveStageUnlock(int stageIndex)
    {
        string key = keyPrefs + stageIndex; // Gabungkan key prefix dengan index stage
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save(); // Pastikan perubahan disimpan
        Debug.Log($"Saved: {key} = 1");
    }

    // Method untuk memuat / load status unlock stage yang sudah disimpan
    // Digunakan ketika Start
    private bool LoadStageUnlock(int stageIndex)
    {
        string key = keyPrefs + stageIndex; // Gabungkan key prefix dengan index stage
        bool isUnlocked = PlayerPrefs.GetInt(key, 0) == 1; // Default 0 jika tidak ditemukan
        Debug.Log($"Loaded: {key} = {isUnlocked}");
        return isUnlocked;
    }
}
