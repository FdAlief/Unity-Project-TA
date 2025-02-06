using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [Header("Store Settings")]
    [SerializeField] private int upgradeTurnCost = 10; // Biaya untuk menambah 1 maxTurn

    // Method untuk membeli upgrade maxTurns
    // Digunakan pada Button pada Panel Store
    public void BuyMaxTurnUpgrade()
    {
        if (CoinManager.Instance.GetTotalCoins() >= upgradeTurnCost)
        {
            // Kurangi koin
            CoinManager.Instance.ReduceCoins(upgradeTurnCost);

            // Tambah maxTurns
            TurnScript.Instance.IncreaseMaxTurns(1);

            Debug.Log("Max Turns Ditambahkan! Sekarang: " + TurnScript.Instance.GetMaxTurns());
        }
        else
        {
            Debug.LogWarning("Koin tidak cukup untuk membeli upgrade!");
        }
    }
}
