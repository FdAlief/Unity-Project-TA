using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIScript : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text totalCoinsText; // UI untuk total koin

    private void Start()
    {
        UITextCoin();
    }

    // Method untuk mendapatkan data nilai Total Coin pada CoinManager
    private void UITextCoin()
    {
        // Subscribe ke event CoinManager
        CoinManager.Instance.OnCoinChanged += UpdateUICoin;

        // Set UI Text Coin
        UpdateUICoin(CoinManager.Instance.GetTotalCoins());
    }

    // Method untuk memperbarui UI Text saat total coin berubah
    private void UpdateUICoin(int totalCoins)
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = totalCoins.ToString();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe saat objek dihancurkan untuk mencegah memory leak
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged -= UpdateUICoin;
        }
    }
}
