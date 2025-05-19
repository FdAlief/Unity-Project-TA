using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIScript : MonoBehaviour
{
    [Header("Counter Number")]
    public CounterNumber coinEffect;
    public CounterNumber turnEffect;

    private void Start()
    {
        UITextCoin();
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

    // Method untuk menghitung Coin yang ditampilkan beserta Effect
    private void UpdateUICoin(int totalCoins)
    {
        if (coinEffect != null)
        {
            coinEffect.EffectToValue(totalCoins);
        }
    }

    // Method untuk mendapatkan data nilai Turn Count pada TurnScript
    private void UITextTurn()
    {
        if (TurnScript.Instance != null)
        {
            TurnScript.Instance.OnTurnChanged += UpdateUITurn;
            UpdateUITurn(TurnScript.Instance.GetTurnCount()); // Set nilai awal
        }
    }

    // Method untuk menghitung Turn yang ditampilkan beserta Effect
    private void UpdateUITurn(int turnCount)
    {
        if (turnEffect != null && TurnScript.Instance != null)
        {
            int maxTurns = TurnScript.Instance.GetMaxTurns();
            int remainingTurns = Mathf.Max(maxTurns - turnCount, 0);
            turnEffect.EffectToValue(remainingTurns);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe saat objek dihancurkan untuk mencegah memory leak
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged -= UpdateUICoin;
        }

        if (TurnScript.Instance != null)
        {
            TurnScript.Instance.OnTurnChanged -= UpdateUITurn;
        }
    }
}
