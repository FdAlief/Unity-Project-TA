using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("Turn Store Settings")]
    [SerializeField] private int upgradeTurnCost = 10; // Biaya untuk menambah 1 maxTurn

    [Header("Special Seed Store Settings")]
    [SerializeField] private SeedConfig seedConfig; // ScriptableObject yang berisi daftar seed
    [SerializeField] private TMP_Text seedNameUI; // UI untuk text nama seed
    //[SerializeField] private Image seedImageUI; // UI untuk menampilkan gambar seed
    //[SerializeField] private Text seedPriceUI; // UI untuk harga seed

    private SeedSpecialData selectedSeed; // Seed yang dipilih secara random

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

    // Method untuk pilih 1 seed secara random dari seedStoreList (SO)
    private void SelectRandomSeed()
    {
        if (seedConfig.seedStoreList.Count > 0)
        {
            int randomIndex = Random.Range(0, seedConfig.seedStoreList.Count);
            selectedSeed = seedConfig.seedStoreList[randomIndex];

            // Update UI
            //seedImageUI.sprite = selectedSeed.seedImage;
            seedNameUI.text = selectedSeed.seedName;
            //seedPriceUI.text = "$" + selectedSeed.price;
        }
        else
        {
            Debug.LogWarning("Store kosong, tidak ada seed yang tersedia!");
        }
    }

    // Method untuk membeli Seed yang sedang ditampilkan dan memmasukkannya ke Data List specialSeedPrefabs (SO)
    // Digunakan pada Button Beli Special Seed di Panel Store
    public void BuySpecialSeed()
    {
        if (selectedSeed != null && CoinManager.Instance.GetTotalCoins() >= selectedSeed.price)
        {
            // Kurangi koin
            CoinManager.Instance.ReduceCoins(selectedSeed.price);

            // Tambahkan ke specialSeedPrefabs
            seedConfig.specialSeedPrefabs.Add(selectedSeed.seedPrefab);

            Debug.Log("Seed Dibeli: " + selectedSeed.seedName);

            // Setelah pembelian, tampilkan seed lain secara random
            SelectRandomSeed();
        }
        else
        {
            Debug.LogWarning("Koin tidak cukup untuk membeli seed ini!");
        }
    }
}
