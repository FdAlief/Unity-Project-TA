using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance; // Singleton agar mudah diakses

    [Header("Stage Objective")]
    [SerializeField] private int[] targetScore; // Skor yang harus dicapai untuk menyelesaikan stage
    [HideInInspector] public bool isObjectiveComplete = false; // Sinyal Apakah semua objective sudah tercapai
    [HideInInspector] public bool isFinalTargetReached = false; // Indikator apakah target terakhir (semua objective) sudah tercapai
    private int currentTargetIndex; // Indeks target skor yang sedang dicapai
    private int[] originalTargetScore; // Menyimpan nilai asli targetScore
    public TMP_Text targetScoreText; // UI Text untuk menampilkan target score

    [Header("Level Index Saat Ini")]
    [SerializeField] private int currentLevelIndex; // indikator index Level untuk di Level Manager

    [Header("Next Level Progress")]
    [SerializeField] private int nextLevelCompletedIndex; // Indeks level yang akan diaktifkan di LevelManager

    [Header("Coin Rewards")]
    [SerializeField] private int[] coinRewards; // Jumlah koin yang diberikan setiap kali target skor tercapai
    private int lastRewardCoins = 0; // Menyimpan reward coin terakhir

    [Header("Win Condition")]
    public GameObject PanelWin; // Panel Win ketika targetScore diraih

    [Header("Lose Condition")]
    public GameObject PanelLose; // Panel Lose ketika targetScore tidak dapat diraih dengan TurnCount

    [Header("Script Enable & Disable")]
    public MonoBehaviour[] scriptDisable; // Untuk menonaktifkan & aktifkan sistem Raycast ketika Panel Win/Lose muncul

    [Header("Effect Angka UI")]
    [SerializeField] private CounterNumber coinEffect;

    [Header("Referensi Script")]
    [SerializeField] private CongklakManager congklakManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ColliderHoleManager colliderHoleManager;
    [SerializeField] private WinScript winScript;
    [SerializeField] private LoseScript loseScript;
    [SerializeField] private StageInput stageInput;
    [SerializeField] private SpecialSeedHandler specialSeedHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        originalTargetScore = (int[])targetScore.Clone(); // Copy nilai Target Score aslinya
        UpdateTargetScoreUI();
    }

    // Update akan mengecek skor setiap frame
    private void Update()
    {
        CheckObjective();
    }

    // Method untuk mengecek apakah objective sudah tercapai
    public void CheckObjective()
    {
        if (!isObjectiveComplete && ScoreManager.Instance != null)
        {
            if (currentTargetIndex < targetScore.Length && ScoreManager.Instance.GetCurrentScore() >= targetScore[currentTargetIndex])
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

        // Simpan stage yang barusan diselesaikan
        int completedIndex = currentTargetIndex;
        if (stageInput != null)
        {
            if (completedIndex >= 0)
            {
                stageInput.MarkStageAsCompleted(completedIndex); // Menandakan stage selesai
            }

            int nextStageIndex = completedIndex + 1;
            if (nextStageIndex < targetScore.Length) // Pastikan tidak lewat batas
            {
                stageInput.UnlockNextStage(nextStageIndex); // Aktifkan stage berikutnya
                stageInput.SaveStageUnlock(nextStageIndex); // Simpan status unlock untuk stage berikutnya
            }
        }

        currentTargetIndex++; // Naik ke target berikutnya

        if (currentTargetIndex < targetScore.Length)
        {
            UpdateTargetScoreUI();
        }
        else
        {
            // Semua target selesai
            isFinalTargetReached = true;

            // Periksa apakah level ini adalah level 0
            if (currentLevelIndex == 0)
            {
                // Paksa aktifkan level 0 di LevelManager
                LevelManager.Instance.CompleteLevel(0);
                Debug.Log("Level 0 telah diselesaikan! Status unlocked diaktifkan.");
            }
            else
            {
                // Simpan level berdasarkan index normal
                LevelManager.Instance.CompleteLevel(currentLevelIndex);
            }

            // Setelah selesai, UNLOCK next level berdasarkan nextLevelCompletedIndex
            if (nextLevelCompletedIndex >= 0 && nextLevelCompletedIndex < LevelManager.Instance.levelUnlocked.Length)
            {
                LevelManager.Instance.levelUnlocked[nextLevelCompletedIndex] = true;
                LevelManager.Instance.SaveLevelProgress(); // Pastikan langsung save ke PlayerPrefs
                Debug.Log($"Level berikutnya ({nextLevelCompletedIndex}) sudah di-UNLOCK!");
            }
            else
            {
                Debug.LogWarning("Next Level Completed Index di luar batas! Cek nextLevelCompletedIndex di Inspector.");
            }

            Debug.Log("Semua objective selesai! Level diselesaikan.");
            return;
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

    // Method untuk mengambil TargetScore saat ini berdasarkan index
    // Digunakan pada script LoseScript (ShowStageOnGameOver) & WinScript (ShowStageWin)
    public int GetCurrentTarget()
    {
        return currentTargetIndex; // Menyesuaikan agar Stage 1 tampil sebagai 1 bukan 0
    }

    // Method untuk mengambil data nilai dari Target Score
    // Digunakan pada Script SpecialSeedHandler (PatungSurabayaSpecialSeed)
    public int GetTargetScoreValue()
    {
        if (currentTargetIndex < targetScore.Length)
        {
            return targetScore[currentTargetIndex];
        }
        return 0;
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

    // Method untuk mereset data nilai Target Score ke nilai Awal / Original
    // Digunakan pada Method (BackStageMenu) & (RestartGame)
    private void ResetTargetScoreToOriginal()
    {
        if (originalTargetScore != null && originalTargetScore.Length == targetScore.Length)
        {
            for (int i = 0; i < targetScore.Length; i++)
            {
                targetScore[i] = originalTargetScore[i];
            }
            UpdateTargetScoreUI();
            Debug.Log("Target score berhasil direset ke nilai awal.");
        }
    }

    // Method untuk menampilkan TargetScore pada UI
    // Digunakan pada method Start & CheckObjective dan lainnya
    private void UpdateTargetScoreUI()
    {
        if (targetScoreText != null && currentTargetIndex < targetScore.Length)
        {
            targetScoreText.text = $"{targetScore[currentTargetIndex]}";
        }
    }

    // Method ini untuk mereset Game ketika Restart pada Pause Menu
    // Digunakan pada Button pada Restart di Pause Menu dan Button BackStage Menu
    public void RestartGame()
    {
        // Aktifkan script yang terdaftar (misalnya Raycast atau kontrol)
        foreach (MonoBehaviour script in scriptDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        // Mereset data TurnCount
        TurnScript.Instance.ResetTurnCount();

        // Menjalankan Effect Count dan Shaking,Rotate,Scale
        coinEffect.EffectToValue(CoinManager.Instance.GetTotalCoins());

        ResetTargetScoreToOriginal(); // Reset Target Score menjadi Awal / Original
    }
}
