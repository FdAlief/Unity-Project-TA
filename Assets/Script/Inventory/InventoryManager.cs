using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Slot Inventory")]
    public GameObject[] inventorySlots; // Array slot UI untuk inventory
    public List<GameObject> seedsInSlots = new List<GameObject>(); // Database biji dalam list dinamis

    [Header("Special Seed Slot Inventory")]
    public GameObject[] specialSeedSlots; // Array slot untuk special seed

    [Header("Special Seed Data")]
    public SeedConfig seedConfig; // Referensi ke SeedConfig ScriptableObject
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector
    private int selectedSpecialSeedSlotIndex = -1; // -1 artinya belum ada yang dipilih

    [Header("ScrollView")]
    public RectTransform contentRect;  // Referensi RectTransform dari Content di ScrollView
    public float slotWidth;    // Tinggi setiap slot
    public float spacing;        // Spasi antar slot

    [Header("Delete Special Seed")]
    public Button[] deleteButtons;
    public GameObject panelDeleteSeed;
    public TMP_Text specialSeedName;
    public TMP_Text specialSeedPrice;
    private int currentDiscountedPrice = 0; // Simpan harga jual sementara

    [Header("Counter Number")]
    public TMP_Text refundCoin;
    public CounterNumber refundEffect;

    [Header("Referensi Script")]
    [SerializeField] private AudioManagerScript audioManager;
    [SerializeField] private GameplayUIScript gameplayUIScript;

    void Start()
    {
        // Cek apakah ada flag dari MainMenu yang menyuruh reload data Inventory
        if (PlayerPrefs.HasKey("ShouldReloadInventory"))
        {
            // Jalankan method untuk reload ulang data Special Seed dari PlayerPrefs
            ReloadSpecialSeed();

            // Hapus flag setelah selesai dipakai, supaya tidak terulang lagi saat scene di-reload
            PlayerPrefs.DeleteKey("ShouldReloadInventory");
            PlayerPrefs.Save();

            Debug.Log("Special Seed di-reload ulang karena flag restart aktif.");
        }

        // Load data
        LoadSpecialSeeds();

        // Set semua slot menjadi tidak aktif jika kosong prefab biji
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetActive(false);
        }

        // Ketika Awal akan menampilkan Slot Special Inventory sesuai data dari script SeedConfig (specialSeedPrefabs)
        AddSpecialSeedsToInventory();

        UpdateContentSize();
    }

    void Update()
    {
        UpdateDeleteButtons();
    }

    // Method untuk memasukkan biji ke dalam slot inventory yang ada
    // Digubakan pada script Congklak Hole (TransferSeedsToInventory)
    public bool AddSeedToInventory(GameObject seed)
    {
        // Panggil Audio
        if (audioManager != null)
        {
            audioManager.PlayAudioByIndex(5);
        }
        else
        {
            Debug.LogWarning("AudioManager belum di-assign di inspector!");
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].activeSelf) // Jika slot kosong
            {
                seedsInSlots.Add(seed); // Tambahkan ke list
                inventorySlots[i].SetActive(true); // Aktifkan slot
                PlaceSeedInSlot(seed, inventorySlots[i]); // Tempatkan seed
                StartCoroutine(TimeToChangeLayer(seed)); // Ubah layer
                UpdateContentSize();
                Debug.Log($"Seed {seed.name} ditambahkan ke slot {i}.");
                return true;
            }
        }

        Debug.LogWarning("Inventory penuh! Tidak ada slot yang tersedia.");
        return false; // Jika inventory penuh
    }

    // Method untuk menghapus data Seed pada Slot Inventory
    // Digunakan pada script DragHandler (HandleDrag - GetMouseUp/Touch Ended)
    public void RemoveSeedFromInventory(GameObject seed)
    {
        if (seedsInSlots.Contains(seed))
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].activeSelf && inventorySlots[i].transform.GetChild(0).gameObject == seed)
                {
                    seedsInSlots.Remove(seed); // Hapus dari list
                    inventorySlots[i].SetActive(false); // Nonaktifkan slot
                    ChangeSeedLayer(seed, "Default"); // Ubah layer
                    seed.transform.SetParent(null); // Lepaskan parent
                    UpdateContentSize();
                    Debug.Log($"Seed {seed.name} dihapus dari slot {i}.");
                    return;
                }
            }
        }
        else
        {
            Debug.LogWarning($"Seed {seed.name} tidak ditemukan di inventory.");
        }
    }

    // Method untuk memasukkan prefab seed dari data List specialSeedPrefabs di Script SeedConfig
    // Digunakan pada Script StoreManager (BuySpecialSeed) & Method Start
    public void AddSpecialSeedsToInventory()
    {
        // Loop melalui daftar specialSeedPrefabs dalam SeedConfig
        for (int i = 0; i < seedConfig.specialSeedPrefabs.Count; i++)
        {
            // Pastikan prefab seed tidak null
            if (seedConfig.specialSeedPrefabs[i] != null)
            {
                // Pastikan slot masih tersedia
                if (i < specialSeedSlots.Length)
                {
                    // Cek apakah slot sudah memiliki seed
                    if (specialSeedSlots[i].transform.childCount > 1) // Slot sudah berisi seed
                    {
                        Debug.Log($"Slot {i} sudah berisi {specialSeedSlots[i].transform.GetChild(1).name}, tidak perlu instantiate lagi.");
                        continue; // Lewati loop agar tidak menduplikasi seed
                    }

                    // Buat instance dari prefab seed sebelum menempatkannya ke slot
                    GameObject seedInstance = Instantiate(seedConfig.specialSeedPrefabs[i]);

                    // Tempatkan seed yang telah di-instantiate ke dalam slot
                    PlaceSeedInSlot(seedInstance, specialSeedSlots[i]);
                }
                else
                {
                    Debug.LogWarning("Tidak cukup slot untuk semua special seeds.");
                    break; // Hentikan loop jika tidak ada slot yang tersedia
                }
            }
        }
    }

    // Method untuk menghapus data Special Seed dari slot tertentu
    // Menggunakan parameter index dari Method ConfirmDeleteSpecialSeed
    private void RemoveSpecialSeedFromInventory(int slotIndex)
    {
        // Cek apakah index valid dalam batas array specialSeedSlots
        if (slotIndex < 0 || slotIndex >= specialSeedSlots.Length)
        {
            Debug.LogWarning($"Index {slotIndex} di luar batas slot.");
            return;
        }

        GameObject slot = specialSeedSlots[slotIndex]; // Ambil slot sesuai index

        // Pastikan slot aktif dan memiliki seed untuk dihapus
        if (slot.activeSelf && slot.transform.childCount > 1)
        {
            Transform seedInSlot = slot.transform.GetChild(1); // Ambil objek seed

            if (seedInSlot != null)
            {
                GameObject seedObject = seedInSlot.gameObject;

                // Cari index seed dalam daftar specialSeedPrefabs berdasarkan nama asli (tanpa "(Clone)")
                int prefabIndex = seedConfig.specialSeedPrefabs.FindIndex(prefab =>
                    prefab.name == seedObject.name.Replace("(Clone)", "").Trim());

                if (prefabIndex != -1)
                {
                    Debug.Log($"Menghapus {seedConfig.specialSeedPrefabs[prefabIndex].name} dari database.");
                    seedConfig.specialSeedPrefabs.RemoveAt(prefabIndex); // Hapus dari database specialSeedPrefabs
                }
                else
                {
                    Debug.LogWarning($"Prefab {seedObject.name} tidak ditemukan dalam specialSeedPrefabs.");
                }

                // Hapus seed dari slot dan hapus objek dari scene
                Destroy(seedObject);

                // Memindahkan seed yang berada di slot bawah ke slot atas jika ada
                ShiftSpecialSeedsUp();

                Debug.Log($"Special Seed di slot {slotIndex} telah dihapus dari inventory.");
            }
        }
        else
        {
            Debug.LogWarning($"Slot {slotIndex} kosong atau tidak memiliki seed untuk dihapus.");
        }
    }

    // Method untuk menggeser seed ke atas jika ada slot kosong di atasnya
    // Digunakan pada method RemoveSpecialSeedFromInventory ketika sudah menghapus data dari Slot
    public void ShiftSpecialSeedsUp()
    {
        if (selectedSpecialSeedSlotIndex == -1) return; // Kalau gak ada yang dipilih, keluar aja.

        for (int i = selectedSpecialSeedSlotIndex; i < specialSeedSlots.Length - 1; i++)
        {
            // Kalau slot ini kosong
            if (specialSeedSlots[i].transform.childCount <= 2)
            {
                for (int j = i + 1; j < specialSeedSlots.Length; j++)
                {
                    if (specialSeedSlots[j].transform.childCount > 1)
                    {
                        // Pindahkan seed dari slot bawah ke slot kosong
                        Transform seedToMove = specialSeedSlots[j].transform.GetChild(1);

                        seedToMove.SetParent(specialSeedSlots[i].transform);
                        seedToMove.localPosition = Vector3.zero;
                        seedToMove.localRotation = Quaternion.identity;
                        seedToMove.localScale = new Vector3(5f, 5f, 5f);

                        Debug.Log($"[SHIFT] Memindahkan {seedToMove.name} dari slot {j} ke slot {i}.");

                        break; // Setelah pindah, langsung keluar loop dalam
                    }
                }
            }
        }
    }

    // Method untuk menghapus semua biji di inventory
    // Digunakan pada script StageManager ketika selesai stage
    public void ClearInventory()
    {
        for (int i = seedsInSlots.Count - 1; i >= 0; i--)
        {
            GameObject seed = seedsInSlots[i];

            // Hapus biji dari slot
            for (int j = 0; j < inventorySlots.Length; j++)
            {
                if (inventorySlots[j].activeSelf && inventorySlots[j].transform.GetChild(0).gameObject == seed)
                {
                    inventorySlots[j].SetActive(false); // Nonaktifkan slot
                    break;
                }
            }

            seedsInSlots.RemoveAt(i); // Hapus biji dari list
            Destroy(seed); // Hapus biji dari scene
        }

        UpdateContentSize(); // Perbarui ukuran content di ScrollView
        Debug.Log("Inventory telah dikosongkan.");
    }

    // Method untuk meletakkan prefab biji ke dalam slot dan menjadi child dari slot
    // Digunakan pada method AddSeedToInventory
    private void PlaceSeedInSlot(GameObject seed, GameObject slot)
    {
        // Set parent prefab biji menjadi slot inventory
        seed.transform.SetParent(slot.transform);

        // Reset posisi, rotasi, dan skala biji agar sesuai dengan slot
        seed.transform.localPosition = Vector3.zero;
        seed.transform.localRotation = Quaternion.identity;
        seed.transform.localScale = new Vector3(5f, 5f, 5f);
    }

    // Pengecekan Seed berada di Inventory
    // Method yang digunakan pada script DragHandler (HandleDrag)
    // Berfungsi agar prefab biji dapat di drag ketika sudah terdapat pada slot Inventory
    public bool IsSeedInInventory(GameObject seed)
    {
        return seedsInSlots.Contains(seed); // Periksa apakah seed ada dalam list
    }

    // Method untuk mengecek apakah inventory kosong
    // Digunakan pada script CongklakHole (HandleClick)
    // Fungsi agar membatasi mengambil biji dari hole ke Inventory
    // Harus kosong slot inventory baru bisa ambil biji dari Hole
    public bool IsInventoryEmpty()
    {
        return seedsInSlots.Count == 0; // Inventory kosong jika list kosong
    }

    // Method untuk mengecek apakah seed ini adalah biji terakhir yang ada di Inventory (Last Seed)
    // Digunakan pada Script CongklakHole (MonasSpecialSeed) dan DragHandler (GetMouseButtonUp)
    public bool IsLastSeed(GameObject seed)
    {
        return seedsInSlots.Count == 0; // Biji terakhir pada Inventory
    }

    // Method untuk mengubah layer prefab biji
    // Digunakan pada method AddSeedToInventory dan RemoveSeedFromInventory
    private void ChangeSeedLayer(GameObject seed, string layerName)
    {
        if (seed == null) return; // Pastikan seed masih ada sebelum mengubah layer

        // Ubah layer pada seed
        seed.layer = LayerMask.NameToLayer(layerName);
        Debug.Log("Layer biji diubah menjadi: " + layerName);
    }

    // Waktu untuk menjalankan perubahan Layer Biji ketika berada di  dalam Inventory
    IEnumerator TimeToChangeLayer(GameObject seed)
    {
        yield return new WaitForSeconds(0.05f); // Menunggu 0.05 detik
        ChangeSeedLayer(seed, "Seed Layer"); // Ubah Layer
    }

    // Method untuk mengatur size Content pada ScrollView Inventory Slot
    private void UpdateContentSize()
    {
        int activeSlots = 0;

        // Hitung jumlah slot aktif
        foreach (var slot in inventorySlots)
        {
            if (slot.activeSelf)
            {
                activeSlots++;
            }
        }

        // Hitung lebar Content berdasarkan slot aktif
        float contentWidth = (activeSlots * slotWidth) + Mathf.Max(0, (activeSlots - 1) * spacing);

        // Perbarui ukuran Content
        contentRect.sizeDelta = new Vector2(contentWidth, contentRect.sizeDelta.y);
    }

    // Method untuk menon-aktifkan Button Delete ketika Slot Kosong
    // Digunakan pada Method Update
    private void UpdateDeleteButtons()
    {
        for (int i = 0; i < specialSeedSlots.Length; i++)
        {
            if (deleteButtons[i] != null)
            {
                // Slot dianggap kosong kalau child count <= 1 (cuma background)
                bool isSlotFilled = specialSeedSlots[i].transform.childCount > 1;
                deleteButtons[i].interactable = isSlotFilled;
            }
        }
    }

    // Method untuk membuka Pane Delete Special Seed ketika klik Button Delete di setiap Slot sesuai dengan Slot
    // Digunakan pada OnClick setiap Button Delete Slot
    public void OpenPanelDeleteSpecialSeed(GameObject slot)
    {
        panelDeleteSeed.SetActive(true);
        gameplayUIScript.ClosePanelInfoSpecialSeed();

        // Simpan slot index yang dipilih
        selectedSpecialSeedSlotIndex = System.Array.IndexOf(specialSeedSlots, slot);

        // Cek apakah slot punya seed (pastikan child lebih dari background doang)
        if (slot.transform.childCount > 1)
        {
            GameObject seedObject = slot.transform.GetChild(1).gameObject;
            string seedNameRaw = seedObject.name.Replace("(Clone)", "").Trim();

            SeedSpecialData seedData = seedConfig.GetSeedDataByPrefabName(seedNameRaw);

            if (seedData != null)
            {
                specialSeedName.text = seedData.seedName;
                currentDiscountedPrice = seedData.price / 2;
                specialSeedPrice.text = currentDiscountedPrice.ToString();

                Debug.Log($"[INFO] Biji spesial ditemukan: {seedData.seedName}, Harga: {seedData.price}");
            }
            else
            {
                Debug.LogWarning($"[WARNING] Data Seed '{seedNameRaw}' tidak ditemukan di SeedStoreList.");
                specialSeedName.text = "-";
                specialSeedPrice.text = "-";
                currentDiscountedPrice = 0; // Reset kalau datanya null
            }
        }
        else
        {
            Debug.LogWarning("Slot tidak memiliki seed spesial untuk dihapus.");
            specialSeedName.text = "-";
            specialSeedPrice.text = "-";
        }
    }

    // Method untuk Konfirmasi Penghapusan Special Seed
    // Digunakan pada OnClick Button Yes di Panel Delete Special Seed
    public void ConfirmDeleteSpecialSeed()
    {
        if (selectedSpecialSeedSlotIndex != -1)
        {
            RemoveSpecialSeedFromInventory(selectedSpecialSeedSlotIndex);
            SaveSpecialSeeds();

            if (currentDiscountedPrice > 0)
            {
                refundCoin.text = $"+ {currentDiscountedPrice}";

                // Menambhakan Total Coin
                CoinManager.Instance.AddCoins(currentDiscountedPrice);

                // Jalankan Effect
                refundEffect.EffectToShake();
                Debug.Log($"[SUCCESS] Tambahkan {currentDiscountedPrice} koin ke player.");
            }
            else
            {
                Debug.LogWarning("[WARNING] Harga seed tidak valid, tidak ada koin yang ditambahkan.");
            }

            panelDeleteSeed.SetActive(false); // Tutup panel setelah hapus
            selectedSpecialSeedSlotIndex = -1; // Reset pilihan
            currentDiscountedPrice = 0; // Reset harga jual
        }
        else
        {
            Debug.LogWarning("[WARNING] Tidak ada slot yang dipilih untuk dihapus.");
        }
    }

    // Method untuk Save Data Special Seed
    // Digunakan pada Script StoreManager (BuySpecialSeed)
    public void SaveSpecialSeeds()
    {
        // Buat instance data untuk disimpan dalam bentuk JSON
        SpecialSeedSaveData data = new SpecialSeedSaveData();

        // Loop semua prefab yang ada di konfigurasi
        foreach (var prefab in seedConfig.specialSeedPrefabs)
        {
            // Pastikan prefab tidak null sebelum disimpan
            if (prefab != null)
                data.specialSeedPrefabNames.Add(prefab.name); // Simpan hanya nama prefab
        }

        // Konversi objek data ke format JSON string
        string json = JsonUtility.ToJson(data);

        // Simpan JSON string ke PlayerPrefs dengan key tertentu
        PlayerPrefs.SetString(keyPrefs, json);
        PlayerPrefs.Save(); // Commit ke disk

        Debug.Log("[SAVE] Special Seed berhasil disimpan.");
    }

    // Method untuk Load Data Special Seed
    // Digunakan pada Method Start dan Reload
    public void LoadSpecialSeeds()
    {
        // Cek apakah key untuk Special Seed sudah ada di PlayerPrefs
        if (PlayerPrefs.HasKey(keyPrefs))
        {
            // Ambil string JSON dari PlayerPrefs
            string json = PlayerPrefs.GetString(keyPrefs);

            // Deserialize JSON ke objek data
            SpecialSeedSaveData data = JsonUtility.FromJson<SpecialSeedSaveData>(json);

            // Kosongkan list yang ada sekarang sebelum diisi ulang
            seedConfig.specialSeedPrefabs.Clear();

            // Loop nama prefab dan load ulang dari folder Resources/Seeds
            foreach (string prefabName in data.specialSeedPrefabNames)
            {
                // Coba load prefab dari folder Resources
                GameObject prefab = Resources.Load<GameObject>("Seeds/" + prefabName); 

                if (prefab != null)
                    seedConfig.specialSeedPrefabs.Add(prefab); // Tambahkan prefab yang berhasil di-load
                else
                    Debug.LogWarning($"[LOAD] Prefab '{prefabName}' tidak ditemukan di Resources/Seeds.");
            }

            Debug.Log("[LOAD] Special Seed berhasil dimuat.");
        }
    }

    // Method Reload Save Data untuk ketika Restart
    // Digunakan pada Script MainMenuManager (OnClickRestart)
    public void ReloadSpecialSeed()
    {
        // Clear cache list
        seedConfig.specialSeedPrefabs.Clear();

        // Baca ulang dari PlayerPrefs (yang sudah kosong karena direset)
        LoadSpecialSeeds();
    }
}