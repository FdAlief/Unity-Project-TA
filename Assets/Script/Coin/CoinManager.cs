using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text totalCoinsText; // UI untuk total koin

    private int totalCoins; // Menyimpan jumlah total koin

    // Method untuk menambahkan nilai Coin yang didapatkan
    // Digunakan pada script StageManager (OnObjectiveComplete)
    public void AddCoins(int amount)
    {
        totalCoins += amount;

        // Update UI
        UpdateUI();

        Debug.Log($"Koin Ditambahkan: {amount}, Total Koin Sekarang: {totalCoins}");
    }

    // Method untuk menampilkan TotalCoin pada UI Text
    private void UpdateUI()
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"{totalCoins}";
        }
    }
}
