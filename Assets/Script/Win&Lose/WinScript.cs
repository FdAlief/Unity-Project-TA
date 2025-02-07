using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    [Header("Script Enable")]
    public MonoBehaviour[] scriptEnable; // Untuk mengaktifkan kembali sistem Raycast

    [Header("UI Elements")]
    public TMP_Text stageText; // Tambahkan UI untuk menampilkan skor
    public TMP_Text scoreText; // Tambahkan UI untuk menampilkan skor
    public TMP_Text rewardCoinText; // Tampilan UI Text RewardCoin
    public TMP_Text remainingTurnText; // Tampilan UI sisa Turn
    public TMP_Text totalText; // Tampilan UI Total

    [Header("Next Scene Level")]
    public string sceneNextLevel;

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;

    private void Update()
    {
        ShowStageWin();
        ShowScore();
        ShowRewardCoins();
        ShowRemainingTurn();
        ShowTotalCoins();
    }

    // Method untuk menampilkan UI text informasi Stage Win
    // Mengguanakn method GetCurrentTarget() dari script StageManager
    private void ShowStageWin()
    {
        if (stageManager != null && stageText != null)
        {
            stageText.text = $"Kamu Menang di Stage {stageManager.GetCurrentTarget() - 1}";
        }
    }

    // Menampilkan skor yang diraih saat menang
    // Mengguanakn method GetLastScore() dari script ScoreManager
    private void ShowScore()
    {
        if (ScoreManager.Instance != null && scoreText != null)
        {
            int lastScore = ScoreManager.Instance.GetLastScore(); // Ambil skor terakhir
            scoreText.text = $"Kumpulkan Sebanyak {lastScore}";
        }
    }

    // Menampilkan jumlah Reward Coin yang diraih pada Panel Win
    // Menggunakan nilai pada script StageManager (GetLastRewardCoins) 
    private void ShowRewardCoins()
    {
        if (stageManager != null && rewardCoinText != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Ambil nilai reward dari StageManager
            rewardCoinText.text = $"{rewardCoins}";
        }
    }

    // Method untuk menampilkan sisa turn (reward turn coin) di UI
    // Menggunakan method pada Script TurnScript
    private void ShowRemainingTurn()
    {
        if (TurnScript.Instance != null && remainingTurnText != null)
        {
            int sisaTurn = TurnScript.Instance.GetRemainingTurns();
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins();
            remainingTurnText.text = $"{sisaTurn} ({rewardCoinsTurn})";
        }
    }

    // Method untuk menampilkan Total Koin (Reward + Coin Sisa Turn)
    // Menggunakan method pada Script TurnScript & StageManager
    private void ShowTotalCoins()
    {
        if (totalText != null && TurnScript.Instance != null && stageManager != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Reward dari StageManager
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins(); // Reward dari sisa turn

            int totalCoins = rewardCoins + rewardCoinsTurn; // Total keseluruhan

            totalText.text = $"{totalCoins}"; // Update UI
        }
    }

    // Method untuk menambahkan atau memasukkan total coin ke data CoinManager
    // Ketika menyelesaikan objective
    // Digunakan pada script StageManager (OnObjectiveComplete)
    public void AddToCoinManager()
    {
        if (totalText != null && TurnScript.Instance != null && stageManager != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Reward dari StageManager
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins(); // Reward dari sisa turn

            int totalCoins = rewardCoins + rewardCoinsTurn; // Total keseluruhan

            // Memasukkan / menambahkan ke dalam data Total Coin Manager
            CoinManager.Instance.AddCoins(totalCoins);
        }
    }

    // Method untuk mengaktifkan kembali sistem Raycast ketika sudah Win/Lose
    // Digunakan pada Button di Panel Store setelah dari Panel Win
    public void ActiveRaycast()
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }

        // Reset turnCount
        TurnScript.Instance.ResetTurnCount();
    }

    // Method pindah scene ke Level berikutnya ketika menyelesaikan Level sebelumnya
    // Menggunakan Sinyal dari script StageManager
    // Digunakan pada Button Lanjut Store
    public void ChangeSceneOnWinLevel()
    {
        if (stageManager != null && stageManager.isFinalTargetReached)
        {
            SceneManager.LoadScene(sceneNextLevel); // Ganti dengan nama scene tujuan
        }
        else
        {
            Debug.Log("Belum mencapai target terakhir! Tidak bisa pindah scene.");
        }
    }
}
