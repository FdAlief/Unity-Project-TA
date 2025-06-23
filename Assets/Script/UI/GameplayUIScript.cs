using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameplayUIScript : MonoBehaviour
{
    [Header("Effect Angka UI")]
    public CounterNumber coinEffect;
    public CounterNumber turnEffect;

    [Header("Inventory Special Seed")]
    public TMP_Text textInfoJenis;
    public TMP_Text textInfoDeskripsi;
    public Animator animPanelInventory;
    public Animator animPanelInfoSeed;

    [Header("Next Scene Level")]
    public string sceneNextLevel;
    public GameObject panelNextLevel;

    [Header("Referensi Script")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private SFXAudio sfxAudio;

    private void Start()
    {
        UITextCoin();
        UITextTurn();
    }

    // Method untuk Update UI Coin menggunakan data dari CoinManager
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

    // Method untuk memberi Effect pada Coin
    private void UpdateUICoin(int totalCoins)
    {
        if (coinEffect != null)
        {
            coinEffect.EffectToAll(totalCoins);
        }
    }

    // Method untuk Update UI Turn menggunakan data dari Turn Script
    private void UITextTurn()
    {
        if (TurnScript.Instance != null)
        {
            TurnScript.Instance.OnTurnChanged += UpdateUITurn;
            UpdateUITurn(TurnScript.Instance.GetTurnCount()); // Set nilai awal
        }
    }

    // Method untuk menmberi Effect pada Turn
    private void UpdateUITurn(int turnCount)
    {
        if (turnEffect != null && TurnScript.Instance != null)
        {
            int maxTurns = TurnScript.Instance.GetMaxTurns();
            int remainingTurns = Mathf.Max(maxTurns - turnCount, 0);
            turnEffect.EffectToAll(remainingTurns);
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

    /// <summary>
    /// Inventory Special Seed Function
    /// </summary>
    
    // Method untuk Open Panel Inventory Special Seed
    // Digunakan pada Button Open Panel Inventory Special Seed
    public void OpenPanelInventorySpecialSeed()
    {
        animPanelInventory.SetBool("IsOpen", true);
    }

    // Method untuk Close Panel Inventory Special Seed
    // Digunakan pada Script StageManager (RestartGame) dan Button Close Panel Inventory Special Seed
    public void ClosePanelInventorySpecialSeed()
    {
        animPanelInventory.SetBool("IsOpen", false);
        animPanelInfoSeed.SetBool("IsOpenInfo", false);
    }

    // Method untuk Open Panel Info Special Seed
    // Digunakan pada Button Info Panel Inventory Special Seed
    public void OpenPanelInfoSpecialSeed()
    {
        animPanelInfoSeed.SetBool("IsOpenInfo", true);
    }

    // Method untuk mengambil dan menampilkan data Special Seed pada Info Text
    // Dipanggil dari InventoryManager ketika biji spesial di slot berubah.
    public void DataInfoSpecialSeed()
    {
        // Pastikan inventoryManager sudah di-assign
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager belum di-assign di GameplayUIScript!");
            return;
        }

        // Directly reference the single specialSeedSlot from InventoryManager
        GameObject slotToExamine = inventoryManager.specialSeedSlot;

        // Cek apakah slot punya seed (pastikan child hanya berupa prefab special seed)
        // Check for childCount > 0 AND check if the first child is actually an instantiated seed
        if (slotToExamine != null && slotToExamine.transform.childCount > 0 && slotToExamine.transform.GetChild(0).name.Contains("(Clone)"))
        {
            GameObject seedObject = slotToExamine.transform.GetChild(0).gameObject;
            string seedNameRaw = seedObject.name.Replace("(Clone)", "").Trim();

            // Akses seedConfig melalui InventoryManager untuk konsistensi
            SeedSpecialData seedData = inventoryManager.seedConfig.GetSeedDataByPrefabName(seedNameRaw);

            if (seedData != null)
            {
                textInfoJenis.text = seedData.seedName;
                textInfoDeskripsi.text = seedData.seedInfo;

                Debug.Log($"[GameplayUI] Menampilkan Info Biji Spesial: {seedData.seedName}");
            }
            else
            {
                Debug.LogWarning($"[GameplayUI] Data Seed '{seedNameRaw}' tidak ditemukan di SeedConfig.");
                textInfoJenis.text = "-";
                textInfoDeskripsi.text = "Data tidak ditemukan.";
            }
        }
        else
        {
            // Jika slot kosong atau tidak ada biji yang ditampilkan
            Debug.Log("[GameplayUI] Slot special seed kosong atau tidak memiliki seed.");
            textInfoJenis.text = "-";
            textInfoDeskripsi.text = "Tidak ada biji spesial.";
        }
    }

    /// <summary>
    /// Next Level Function
    /// </summary>
    
    // Method untuk pindah scene ke level selanjutnya
    // Digunakan pada Script WinScript (ContinueGame)
    public void NextLevel()
    {
        StartCoroutine(PanelNextLevel());
    }

    // Coroutine untuk mengaktifkan Panel Next Level dahulu baru pindah Scene
    // Digunakan pada Method NextLevel()
    IEnumerator PanelNextLevel()
    {
        panelNextLevel.SetActive(true); // Aktifkan Panel
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneNextLevel); // Ganti dengan nama scene tujuan
    }
}
