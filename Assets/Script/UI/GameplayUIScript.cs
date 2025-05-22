using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayUIScript : MonoBehaviour
{
    [Header("Effect Angka UI")]
    public CounterNumber coinEffect;
    public CounterNumber turnEffect;

    [Header("Info Special Seed")]
    public GameObject[] textdbuttonInfo;
    public Animator animPanelInfo;
    public TMP_Text textInfoJenis;
    public TMP_Text textInfoDeskripsi;
    private int selectedSpecialSeedSlotIndex = -1; // -1 artinya belum ada yang dipilih

    [Header("Seed Data")]
    [SerializeField] private SeedConfig seedConfig;

    [Header("Referensi Script")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private AudioManagerScript audioManager;

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
    /// Info Special Seed Function
    /// </summary>

    // Method untuk digunakan pada Button Inventory Spesial Seed
    public void OpenPanelInfoSpecialSeed(GameObject slot)
    {
        // Cek apakah slot memiliki lebih dari 1 child (artinya ada seed selain background)
        if (slot.transform.childCount > 1)
        {
            // Play SFX Click
            audioManager.PlayAudioByIndex(0);

            StartCoroutine(PlayAnimasiPanelInfo());

            // Optional: langsung update infonya juga
            DataInfoSpecialSeed(slot);
        }
        else
        {
            Debug.LogWarning("Tidak bisa membuka panel, slot kosong atau hanya background.");
        }
    }

    // Method untuk digunakan pada Button Close Panel Info Special Seed
    public void ClosePanelInfoSpecialSeed()
    {
        StartCoroutine(StopAnimasiPanelInfo());
    }

    // Coroutine untuk Play
    // Digunakan pada Method OpenPanelInfoSpecialSeed()
    IEnumerator PlayAnimasiPanelInfo()
    {
        animPanelInfo.SetBool("isOpen", true);

        yield return new WaitForSeconds(1f);

        // Fade in semua tombol info
        foreach (GameObject btn in textdbuttonInfo)
        {
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg != null) StartCoroutine(FadeCanvasGroup(cg, 1f, 0.3f));
        }
    }

    // Coroutine untuk Stop
    // Digunakan pada Method ClosePanelInfoSpecialSeed()
    IEnumerator StopAnimasiPanelInfo()
    {
        // Fade out semua tombol info
        foreach (GameObject btn in textdbuttonInfo)
        {
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg != null) StartCoroutine(FadeCanvasGroup(cg, 0f, 0.3f));
        }

        yield return new WaitForSeconds(0.5f);

        animPanelInfo.SetBool("isOpen", false);
    }

    // Coroutine untuk Fade in / out (Button Close dan Text) di Panel Info Special Seed
    // Digunakan pada Coroutine PlayAnimasiPanelInfo() dan CloseAnimasiPanelInfo()
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.interactable = targetAlpha > 0f;
        cg.blocksRaycasts = targetAlpha > 0f;
    }

    // Method untuk mengambil dan menampilkan data Special Seed pada Info Text
    // Digunakan pada Method OpenPanelInfoSpecialSeed
    private void DataInfoSpecialSeed(GameObject slot)
    {
        // Simpan slot index yang dipilih
        selectedSpecialSeedSlotIndex = System.Array.IndexOf(inventoryManager.specialSeedSlots, slot);

        // Cek apakah slot punya seed (pastikan child lebih dari background doang)
        if (slot.transform.childCount > 1)
        {
            GameObject seedObject = slot.transform.GetChild(1).gameObject;
            string seedNameRaw = seedObject.name.Replace("(Clone)", "").Trim();

            SeedSpecialData seedData = seedConfig.GetSeedDataByPrefabName(seedNameRaw);

            if (seedData != null)
            {
                textInfoJenis.text = seedData.seedName;
                textInfoDeskripsi.text = seedData.seedInfo;
                //currentDiscountedPrice = seedData.price / 2;
                //specialSeedPrice.text = currentDiscountedPrice.ToString();

                Debug.Log($"[INFO] Biji spesial ditemukan: {seedData.seedName}, Harga: {seedData.price}");
            }
            else
            {
                Debug.LogWarning($"[WARNING] Data Seed '{seedNameRaw}' tidak ditemukan di SeedStoreList.");
                textInfoJenis.text = "-";
                textInfoDeskripsi.text = "_";
                //specialSeedPrice.text = "-";
                //currentDiscountedPrice = 0; // Reset kalau datanya null
            }
        }
        else
        {
            Debug.LogWarning("Slot tidak memiliki seed spesial untuk dihapus.");
            textInfoJenis.text = "-";
            textInfoDeskripsi.text = "_";
            //specialSeedPrice.text = "-";
        }
    }
}
