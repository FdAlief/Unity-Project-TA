using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    [Header("Stage Objective")]
    [SerializeField] private int[] targetScore; // Skor yang harus dicapai untuk menyelesaikan stage
    public bool isObjectiveComplete = false; // Apakah objective sudah tercapai
    private int currentTargetIndex; // Indeks target skor yang sedang dicapai
    public TMP_Text targetScoreText; // UI Text untuk menampilkan target score

    [Header("Coin Rewards")]
    [SerializeField] private int[] coinRewards; // Jumlah koin yang diberikan setiap kali target skor tercapai
    private int lastRewardCoins = 0; // Menyimpan reward coin terakhir

    [Header("Win Condition")]
    public GameObject PanelWin; // Panel Win ketika targetScore diraih

    [Header("Win Condition")]
    public GameObject PanelLose; // Panel Lose ketika targetScore tidak dapat diraih dengan TurnCount

    [Header("Script Disable")]
    public MonoBehaviour[] scriptDisable; // Untuk menonaktifkan sistem Raycast ketika Panel Win/Lose muncul

    [Header("Referensi Script")]
    [SerializeField] private CongklakManager congklakManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ColliderHoleManager colliderHoleManager;
    [SerializeField] private WinScript winScript;
    [SerializeField] private LoseScript loseScript;
    [SerializeField] private StageInput stageInput;

    private void Start()
    {
        UpdateTargetScoreUI();
    }

    // Update akan mengecek skor setiap frame
    private void Update()
    {
        CheckObjective();
    }

    // Method untuk mengecek apakah objective sudah tercapai
    private void CheckObjective()
    {
        if (!isObjectiveComplete && ScoreManager.Instance != null)
        {
            if (ScoreManager.Instance.GetCurrentScore() >= targetScore[currentTargetIndex])
            {
                isObjectiveComplete = true;
                OnObjectiveComplete();
            }
        }
    }

    // Aksi yang dilakukan jika objective tercapai
    private void OnObjectiveComplete()
    {
        // Kembalikan menjadi false agar melanjutkan ke objective selanjutnya
        isObjectiveComplete = false;

        // Simpan nilai Reward Coin berdasarkan target score yang tercapai
        lastRewardCoins = coinRewards[currentTargetIndex];

        // Menambahkan total coin manager dari hasil yang didapatkan
        winScript.AddToCoinManager();

        // Aktifkan Panel Win
        PanelWin.SetActive(true);

        // Nonaktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptDisable)
        {
            if (script != null)
            {
                script.enabled = false; // Nonaktifkan script
            }
        }

        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        // Pindah ke targetscore / objective berikutnya jika ada
        if (currentTargetIndex < targetScore.Length)
        {
            currentTargetIndex++;
            UpdateTargetScoreUI();

            // Unlock UI Button stage berikutnya
            if (stageInput != null)
            {
                stageInput.UnlockNextStage(currentTargetIndex);

                // Simpan stage yang sudah dibuka
                stageInput.SaveStageUnlock(currentTargetIndex);  // Menyimpan status stage yang telah dibuka
            }
        }

        Debug.Log("Objective Complete! Target Score Reached!");
    }

    // Method untuk menangani kondisi Game Over
    // Digunakan pada script TurnScript ketika sudah mencapai TurnCount
    public void OnGameOver()
    {
        // Memanggil method pada script LoseScript untuk menampilkan UI text Stage Lose
        loseScript.ShowStageOnGameOver();

        // Reset target score ke array pertama (indeks 0) & Stage Informasi ke (index 1)
        currentTargetIndex = 0;
        UpdateTargetScoreUI();

        // Aktifkan Panel Game Over
        PanelLose.SetActive(true);

        // Nonaktifkan script yang terdaftar (misalnya Raycast atau kontrol)
        foreach (MonoBehaviour script in scriptDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        Debug.Log("Game Over! Turn sudah maksimal tetapi target skor tidak tercapai.");
    }

    // Method untuk Mengambil Reward Coin Terakhir
    // Digunakan pada Script WinScript (ShowRewardCoins)
    public int GetLastRewardCoins()
    {
        return lastRewardCoins;
    }

    // Method untuk mengambil informasi Stage terakhir sesuai dengan TargetScore
    // Digunakan pada script LoseScript (ShowStageOnGameOver)
    public int GetCurrentTarget()
    {
        return currentTargetIndex + 1; // Menyesuaikan agar Stage 1 tampil sebagai 1 bukan 0
    }

    // Method untuk menampilkan TargetScore pada UI
    // Digunakan pada method Start & CheckObjective
    private void UpdateTargetScoreUI()
    {
        if (targetScoreText != null)
        {
            targetScoreText.text = $"{targetScore[currentTargetIndex]}";
        }
    }

    // Method untuk mengatur Target Score / Objective berapa yang dipilih menggunakan Button
    // Digunakan pada script StageInput (ChooseStage)
    public void SetTargetScoreIndex(int index)
    {
        if (index >= 0 && index < targetScore.Length)
        {
            currentTargetIndex = index;
            UpdateTargetScoreUI();
        }
        else
        {
            Debug.LogWarning("Index target score di luar batas array!");
        }
    }

    // Method ini untuk mereset Game Stage ketika kembali ke Panel Stage Menu
    // Digunakan pada Button pada Gameplay Back Stage Menu
    public void BackStageMenu()
    {
        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        // Mereset data TurnCount
        TurnScript.Instance.ResetTurnCount();
    }
}
