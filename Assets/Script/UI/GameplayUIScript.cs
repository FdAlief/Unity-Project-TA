using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIScript : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text totalCoinsText; // UI untuk total koin
    public TMP_Text scoreText; // UI untuk total koin
    public TMP_Text turnCountText; // UI untuk turn count

    private void Start()
    {
        UITextCoin();
        UITextScore();
        UITextTurn();
    }

    // Method untuk mendapatkan data nilai Total Coin pada CoinManager
    private void UITextCoin()
    {
        if (CoinManager.Instance != null)
        {
            // Subscribe ke event CoinManager
            CoinManager.Instance.OnCoinChanged += UpdateUICoin;

            // Set UI Text Coin
            UpdateUICoin(CoinManager.Instance.GetTotalCoins());
        }
    }

    // Method untuk memperbarui UI Text saat total coin berubah
    private void UpdateUICoin(int totalCoins)
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = totalCoins.ToString();
        }
    }

    // Method untuk mendapatkan nilai Score dari ScoreManager
    private void UITextScore()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateUIScore;
            UpdateUIScore(ScoreManager.Instance.GetCurrentScore()); // Set nilai awal
        }
    }

    // Method untuk memperbarui UI Text saat Score berubah
    private void UpdateUIScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    // 🔹 Update UI Turn Count dari TurnScript
    private void UITextTurn()
    {
        if (TurnScript.Instance != null)
        {
            TurnScript.Instance.OnTurnChanged += UpdateUITurn;
            UpdateUITurn(TurnScript.Instance.GetTurnCount()); // Set nilai awal
        }
    }

    private void UpdateUITurn(int turnCount)
    {
        if (turnCountText != null)
        {
            turnCountText.text = $"Turn : {turnCount} / {TurnScript.Instance.GetMaxTurns()}";
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe saat objek dihancurkan untuk mencegah memory leak
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged -= UpdateUICoin;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateUIScore;
        }

        if (TurnScript.Instance != null)
        {
            TurnScript.Instance.OnTurnChanged -= UpdateUITurn;
        }
    }
}
