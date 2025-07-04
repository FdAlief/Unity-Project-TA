using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [Header("Syarat Coin")]
    [SerializeField] private GameObject coinNotEnoughUI; // UI untuk Panel Syarat Coin
    [SerializeField] private GameObject inventoryFullUI; // UI untuk Panel Inventory Full

    [Header("Turn Store Settings")]
    [SerializeField] private int upgradeTurnCost = 10; // Biaya untuk menambah 1 maxTurn
    [SerializeField] private GameObject turnBuyUI; // UI untuk Panel Turn

    [Header("Special Seed Store Settings")]
    [SerializeField] private SeedConfig seedConfig; // ScriptableObject yang berisi daftar seed
    [SerializeField] private TMP_Text seedNameUI; // UI untuk text nama seed
    [SerializeField] private TMP_Text seedInfoUI; // UI untuk text info seed
    [SerializeField] private GameObject seedBuyUI; // UI untuk Panel Buy seed
    //[SerializeField] private TMP_Text seedPriceUI; // UI untuk harga seed
    [SerializeField] private Image seedImageUI; // UI untuk menampilkan gambar seed

    private SeedSpecialData selectedSeed; // Seed yang dipilih secara random

    [Header("Effect Angka UI")]
    public CounterNumber coinStoreEffect;
    public CounterNumber reduceCoinEffect;
    public TMP_Text reduceCoinText;

    [Header("Referensi Script")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private SFXAudio sfxAudio;

    // Ketika Aktif akan Update UI Coin Store menggunakan data dari CoinManager
    void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged += UpdateStoreUICoin;
        }

        UpdateStoreUICoin(CoinManager.Instance.GetTotalCoins());
    }

    // Ketika NonAktif akan Stop Update UI Coin Store menggunakan data dari CoinManager
    void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged -= UpdateStoreUICoin;
        }

        reduceCoinEffect.SetInitialValue(0);
        reduceCoinText.alpha = 0;
    }

    // Method untuk memberi Effect pada Coin Store
    private void UpdateStoreUICoin(int totalCoins)
    {
        if (coinStoreEffect != null)
        {
            coinStoreEffect.EffectToAll(totalCoins);
        }
    }

    // Method untuk membeli upgrade maxTurns
    // Digunakan pada Button pada Panel Store
    public void BuyMaxTurnUpgrade()
    {
        if (CoinManager.Instance.GetTotalCoins() >= upgradeTurnCost)
        {
            // Kurangi koin
            CoinManager.Instance.ReduceCoins(upgradeTurnCost);

            // Effect Reduce Coin
            reduceCoinText.text = $"- {upgradeTurnCost}";
            reduceCoinEffect.EffectToShake();

            // Tambah maxTurns
            TurnScript.Instance.IncreaseMaxTurns(1);

            // Menonaktifkan Panel setelah membeli Turn
            turnBuyUI.SetActive(false);

            // Play SFX
            sfxAudio.PlayAudioByIndex(1);

            Debug.Log("Max Turns Ditambahkan! Sekarang: " + TurnScript.Instance.GetMaxTurns());
        }
        else
        {
            StartCoroutine(ShowCoinNotEnoughUI());
            Debug.LogWarning("Koin tidak cukup untuk membeli upgrade!");
        }
    }

    // Method untuk pilih 1 seed secara random dari seedStoreList (SO)
    // Digunakan pada Button Lanjut di Panel Win sebelum Store
    public void SelectRandomSpecialSeed()
    {
        // Buat list sementara berisi seed yang belum dibeli
        List<SeedSpecialData> availableSeeds = seedConfig.seedStoreList.FindAll(seed => !seedConfig.specialSeedPrefabs.Contains(seed.seedPrefab));

        if (availableSeeds.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSeeds.Count);
            selectedSeed = availableSeeds[randomIndex];

            // Update UI
            seedNameUI.text = selectedSeed.seedName;
            seedInfoUI.text = selectedSeed.seedInfo;
            seedBuyUI.SetActive(true);
            //seedPriceUI.text = selectedSeed.price.ToString();
            seedImageUI.sprite = selectedSeed.seedImage;
        }
        else
        {
            Debug.LogWarning("Semua seed sudah dibeli! Tidak ada seed yang tersedia di store.");
            selectedSeed = null;
            seedBuyUI.SetActive(false);
        }
    }

    // Method untuk membeli Seed yang sedang ditampilkan dan memmasukkannya ke Data List specialSeedPrefabs (SO)
    // Digunakan pada Button Beli Special Seed di Panel Store
    public void BuySpecialSeed()
    {
        // Cek apakah specialSeedPrefabs sudah penuh (maksimal 4 biji)
        if (seedConfig.specialSeedPrefabs.Count >= 4)
        {
            StartCoroutine(ShowInventoryFullUI());
            Debug.LogWarning("Inventory Special Seed penuh! Tidak bisa membeli lagi.");
            return;
        }

        if (selectedSeed != null && CoinManager.Instance.GetTotalCoins() >= selectedSeed.price)
        {
            // Kurangi koin
            CoinManager.Instance.ReduceCoins(selectedSeed.price);

            // Effect Reduce Coin
            reduceCoinText.text = $"- {selectedSeed.price}";
            reduceCoinEffect.EffectToShake();

            // Tambahkan ke specialSeedPrefabs
            seedConfig.specialSeedPrefabs.Add(selectedSeed.seedPrefab);

            // Menonaktifkan Panel setelah membeli special seed
            seedBuyUI.SetActive(false);

            // Menampilkan data Prefab Special Seed pada Slot
            inventoryManager.DisplayCurrentSpecialSeed();

            // Save Data Special Seed
            inventoryManager.SaveSpecialSeeds();

            // Play SFX
            sfxAudio.PlayAudioByIndex(1);

            Debug.Log("Seed Dibeli: " + selectedSeed.seedName);
        }
        else
        {
            StartCoroutine(ShowCoinNotEnoughUI());
            Debug.LogWarning("Koin tidak cukup untuk membeli seed ini!");
        }
    }

    // Method untuk Aktif dan Nonaktif Panel Coin Not Enough
    // Digunakan pada Method Buy
    private IEnumerator ShowCoinNotEnoughUI()
    {
        coinNotEnoughUI.SetActive(true);
        sfxAudio.PlayAudioByIndex(9);

        yield return new WaitForSeconds(1f);

        coinNotEnoughUI.SetActive(false);
    }

    // Method untuk Aktif dan Nonaktif Panel Inventory Full
    // Digunakan pada Method Buy Special Seed
    private IEnumerator ShowInventoryFullUI()
    {
        inventoryFullUI.SetActive(true);
        sfxAudio.PlayAudioByIndex(9);

        yield return new WaitForSeconds(1f);

        inventoryFullUI.SetActive(false);
    }
}
