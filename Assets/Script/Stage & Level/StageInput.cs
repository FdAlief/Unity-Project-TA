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

    [Header("UI Completed")]
    [SerializeField] private GameObject[] completedInfo; // UI indicator "Completed"

    [Header("UI Stage Menu")]
    [SerializeField] private GameObject stageMenuPanel; // Panel Stage Menu

    [Header("Effect Angka UI")]
    [SerializeField] private CounterNumber coinEffect;
    [SerializeField] private CounterNumber turnEffect;

    [Header("PlayerPrefs Key to Unlock Stage")]
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private CongklakManager congklakManager;

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
                bool isUnlocked = (i == 0) || LoadStageUnlock(i);
                bool isCompleted = LoadStageCompleted(i);

                // Stage hanya bisa diakses kalau unlocked dan belum completed
                stageButtons[i].interactable = isUnlocked && !isCompleted;

                // Tampilkan UI completed kalau sudah selesai
                if (completedInfo.Length > i && completedInfo[i] != null)
                {
                    completedInfo[i].SetActive(isCompleted);
                }

                if (i == 0) SaveStageUnlock(0); // Pastikan stage 0 selalu disimpan
            }
        }
    }

    // Method untuk memilih Objective yang dijalankan
    // Digunakan pada UI Button Stage Input Menu
    public void ChooseStage(int index)
    {
        StartCoroutine(ClosePanelStageMenu(index));
    }

    // Courutine untuk memilih Stage
    // Digunakan pada Method ChooseStage();
    private IEnumerator ClosePanelStageMenu(int index)
    {
        stageMenuPanel.SetActive(false);

        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }
        
        // Atur Target Score
        stageManager.SetTargetScoreIndex(index);

        // Reset biji Congklak ketika stage dipilih untuk random ulang
        congklakManager.ResetSeeds();

        yield return new WaitForSeconds(0.05f);

        // Jalankan Efeect Counter dan Shake,Rotate,Scale
        coinEffect.EffectToAll(CoinManager.Instance.GetTotalCoins());
        turnEffect.EffectToAll(TurnScript.Instance.GetMaxTurns());
    }

    // Method ini untuk mengaktifkan Button Stage selanjutnya
    // Digunakan dan dipanggil oleh StageManager (OnObjectiveComplete) ketika stage objective selesai
    public void UnlockNextStage(int stageIndex)
    {
        if (stageIndex < stageButtons.Length)
        {
            stageButtons[stageIndex].interactable = true;

            // Pastikan hanya stage yang sudah selesai yang menampilkan UI Completed
            if (completedInfo.Length > stageIndex && completedInfo[stageIndex] != null)
            {
                bool isCompleted = LoadStageCompleted(stageIndex);
                completedInfo[stageIndex].SetActive(isCompleted); // Hanya aktif jika stage selesai
            }

            // Simpan ke PlayerPrefs untuk status stage
            SaveStageUnlock(stageIndex);
        }
        else
        {
            Debug.LogWarning("StageIndex di luar jangkauan button/ UI Completed.");
        }
    }

    // Method untuk menyimpan status unlock stage playerprefs
    // Digunakan dan dipanggil oleh StageManager (OnObjectiveComplete) ketika stage objective selesai
    public void SaveStageUnlock(int stageIndex)
    {
        string key = keyPrefs + "Unlock_" + stageIndex; // Gabungkan key prefix dengan index stage
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save(); // Pastikan perubahan disimpan
        Debug.Log($"Saved: {key} = 1");
    }

    // Method untuk memuat / load status unlock stage yang sudah disimpan
    // Digunakan ketika Start
    private bool LoadStageUnlock(int stageIndex)
    {
        string key = keyPrefs + "Unlock_" + stageIndex; // Gabungkan key prefix dengan index stage
        bool isUnlocked = PlayerPrefs.GetInt(key, 0) == 1; // Default 0 jika tidak ditemukan
        Debug.Log($"Loaded: {key} = {isUnlocked}");
        return isUnlocked;
    }

    // Method untuk menyimpan UI Compelted stage playerprefs
    // Digunakan ketika pada Method MarkStageAsCompleted
    private void SaveStageCompleted(int stageIndex)
    {
        string key = keyPrefs + "Completed_" + stageIndex;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"Stage Completed Saved: {key} = 1");
    }

    // Method untuk memuat / load status UI Completed stage yang sudah disimpan
    // Digunakan ketika pada Method Start dan UnlockedStage
    private bool LoadStageCompleted(int stageIndex)
    {
        string key = keyPrefs + "Completed_" + stageIndex;
        bool completed = PlayerPrefs.GetInt(key, 0) == 1;
        Debug.Log($"Stage Completed Loaded: {key} = {completed}");
        return completed;
    }

    // Method untuk menandakan UI Completed Stage dan Disbale Button Stage ketika Completed
    // Digunakan pada Script StageManager (OnObjectiveCompleted)
    public void MarkStageAsCompleted(int stageIndex)
    {
        // Mengaktifkan UI Info Completed ketika Stage Completed
        if (completedInfo.Length > stageIndex && completedInfo[stageIndex] != null)
        {
            completedInfo[stageIndex].SetActive(true);
        }

        // Disbale Stage Button ketika Stage Completed
        if (stageButtons.Length > stageIndex && stageButtons[stageIndex] != null)
        {
            stageButtons[stageIndex].interactable = false;
        }

        SaveStageCompleted(stageIndex);
    }
}
