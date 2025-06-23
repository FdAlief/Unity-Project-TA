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
    public GameObject specialSeedSlot; // Array slot untuk special seed

    [Header("Special Seed Data")]
    public SeedConfig seedConfig; // Referensi ke SeedConfig ScriptableObject
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector
    private int currentSpecialSeedIndex = 0; // Track the currently displayed special seed index

    [Header("ScrollView")]
    public RectTransform contentRect;  // Referensi RectTransform dari Content di ScrollView
    public float slotWidth;    // Tinggi setiap slot
    public float spacing;        // Spasi antar slot

    [Header("Delete Special Seed")]
    public Button deleteButton;
    public Button infoButton;
    public GameObject panelDeleteSeed;
    public TMP_Text specialSeedName;
    public TMP_Text specialSeedPrice;
    public Image seedImageUI;
    private int currentDiscountedPrice = 0; // Simpan harga jual sementara

    [Header("Navigation Buttons")]
    public Button nextSpecialSeedButton;
    public Button previousSpecialSeedButton;

    [Header("Counter Number")]
    public TMP_Text refundCoin;
    public CounterNumber refundEffect;

    [Header("Referensi Script")]
    [SerializeField] private SFXAudio sfxAudio;
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

        DisplayCurrentSpecialSeed(); // Initialize the special seed slot display
        UpdateNavigationButtons(); // Update button states after initial display

        UpdateContentSize();
    }

    private void Update()
    {
        UpdateDeleteInfoButton(); // Cek Button Delete & Info
    }

    // Method untuk memasukkan biji ke dalam slot inventory yang ada
    // Digubakan pada script Congklak Hole (TransferSeedsToInventory)
    public bool AddSeedToInventory(GameObject seed)
    {
        // Panggil Audio
        if (sfxAudio != null)
        {
            sfxAudio.PlayAudioByIndex(5);
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

    // Method untuk menjalankan DisplaySeedAndRefreshInfoCoroutine
    // Berfungsi untuk menampilkan data Prefab Special Seed pada Slot Special Seed
    // Digunakan pada Method (Start, RemoveSpecialSeedFromInventory, PreviousSpecialSeed, NextSpecialSeed)
    // dan Script StoreManager (BuySpecialSeed)
    public void DisplayCurrentSpecialSeed()
    {
        StartCoroutine(DisplaySeedAndRefreshInfoCoroutine());
    }

    // Coroutine untuk menjalankan fungsi display prefab special seed pada slot
    // Digunakan pada Method DisplayCurrentSpecialSeed
    private IEnumerator DisplaySeedAndRefreshInfoCoroutine()
    {
        // Hapus seed yang ada di slot saat ini
        if (specialSeedSlot.transform.childCount > 0)
        {
            for (int i = specialSeedSlot.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = specialSeedSlot.transform.GetChild(i).gameObject;
                if (child.name.Contains("(Clone)"))
                {
                    Destroy(child);
                }
            }
        }

        // Beri jeda singkat setelah Destroy untuk memastikan objek benar-benar hilang dari hierarki
        // sebelum kita mencoba menambahkan yang baru dan membaca child count.
        yield return null; // Tunggu 1 frame

        // Tampilkan biji sesuai currentSpecialSeedIndex
        if (seedConfig.specialSeedPrefabs.Count > 0 && currentSpecialSeedIndex >= 0 && currentSpecialSeedIndex < seedConfig.specialSeedPrefabs.Count)
        {
            GameObject seedPrefab = seedConfig.specialSeedPrefabs[currentSpecialSeedIndex];
            if (seedPrefab != null)
            {
                GameObject seedInstance = Instantiate(seedPrefab);
                SetSeedParticleActive(seedInstance, false);
                PlaceSeedInSlot(seedInstance, specialSeedSlot);
                Debug.Log($"Menampilkan Biji Spesial: {seedPrefab.name} pada indeks {currentSpecialSeedIndex}");
            }
        }
        else
        {
            Debug.Log("Tidak ada biji spesial untuk ditampilkan atau indeks di luar batas.");
        }

        UpdateDeleteInfoButton();
        UpdateNavigationButtons();

        // Panggil metode refresh info UI di sini, setelah biji diasumsikan telah ditampilkan
        // Beri jeda lagi untuk memastikan UI sepenuhnya diperbarui setelah instantiate.
        yield return null; // Tunggu 1 frame lagi

        if (gameplayUIScript != null)
        {
            gameplayUIScript.DataInfoSpecialSeed();
        }
    }

    // Method untuk menghapus data Special Seed dari slot tertentu
    // Menggunakan parameter index dari Method ConfirmDeleteSpecialSeed
    private void RemoveSpecialSeedFromInventory()
    {
        GameObject slot = specialSeedSlot;

        // Periksa childCount > 0 untuk memastikan ada isi di slot
        if (slot.activeSelf && specialSeedSlot.transform.childCount > 0)
        {
            GameObject seedObject = null;
            
            // Cari objek biji yang di-instantiate di dalam slot
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = slot.transform.GetChild(i).gameObject;
                if (child.name.Contains("(Clone)")) // Identifikasi biji yang di-instantiate
                {
                    seedObject = child;
                    break;
                }
            }

            if (seedObject != null)
            {
                string seedNameRaw = seedObject.name.Replace("(Clone)", "").Trim();

                // Hapus dari seedConfig.specialSeedPrefabs berdasarkan indeks yang sedang ditampilkan
                if (currentSpecialSeedIndex >= 0 && currentSpecialSeedIndex < seedConfig.specialSeedPrefabs.Count)
                {
                    Debug.Log($"Menghapus {seedConfig.specialSeedPrefabs[currentSpecialSeedIndex].name} dari database.");

                    // Hapus biji yang sedang ditampilkan
                    seedConfig.specialSeedPrefabs.RemoveAt(currentSpecialSeedIndex); 
                }
                else
                {
                    Debug.LogWarning($"Mencoba menghapus biji pada indeks tidak valid {currentSpecialSeedIndex}. Nama prefab: {seedNameRaw}");
                }

                Destroy(seedObject); // Hancurkan GameObject biji di scene

                // Sesuaikan currentSpecialSeedIndex setelah penghapusan
                if (seedConfig.specialSeedPrefabs.Count > 0)
                {
                    // Jika biji yang dihapus adalah biji terakhir, pindah ke indeks sebelumnya
                    if (currentSpecialSeedIndex >= seedConfig.specialSeedPrefabs.Count)
                    {
                        currentSpecialSeedIndex = seedConfig.specialSeedPrefabs.Count - 1;
                    }
                }
                else
                {
                    currentSpecialSeedIndex = 0; // Tidak ada biji tersisa
                }

                DisplayCurrentSpecialSeed(); // Perbarui tampilan untuk menunjukkan biji yang baru
                Debug.Log($"Biji Spesial di slot spesial telah dihapus dari inventory.");
            }
        }
        else
        {
            Debug.LogWarning($"Slot spesial kosong atau tidak memiliki biji untuk dihapus.");
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

    // Method untuk menon-aktifkan Button Delete & Info ketika Slot Kosong
    // Digunakan pada Method Update
    private void UpdateDeleteInfoButton()
    {
        if (deleteButton != null && infoButton != null)
        {
            // Slot dianggap kosong kalau child count <= 0 (kosong)
            bool isSlotFilled = specialSeedSlot.transform.childCount > 0;
            deleteButton.interactable = isSlotFilled;
            infoButton.interactable = isSlotFilled;
        }
    }

    // Method to Update the state of Next/Previous buttons
    // Digunakan pada Method Start & Coroutine DisplaySeedAndRefreshInfoCoroutine
    private void UpdateNavigationButtons()
    {
        if (nextSpecialSeedButton != null)
        {
            nextSpecialSeedButton.interactable = (currentSpecialSeedIndex < seedConfig.specialSeedPrefabs.Count - 1);
        }
        if (previousSpecialSeedButton != null)
        {
            previousSpecialSeedButton.interactable = (currentSpecialSeedIndex > 0);
        }
    }

    // Method Go to the next special seed
    // Digunakan pada Button Next di Panel Inventory Special Seed
    public void NextSpecialSeed()
    {
        if (currentSpecialSeedIndex < seedConfig.specialSeedPrefabs.Count - 1)
        {
            currentSpecialSeedIndex++;
            DisplayCurrentSpecialSeed();
            Debug.Log($"Moving to next special seed: Index {currentSpecialSeedIndex}");
        }
    }

    // Method Go to the previous special seed
    // Digunakan pada Button Previous di Panel Inventory Special Seed
    public void PreviousSpecialSeed()
    {
        if (currentSpecialSeedIndex > 0)
        {
            currentSpecialSeedIndex--;
            DisplayCurrentSpecialSeed();
            Debug.Log($"Moving to previous special seed: Index {currentSpecialSeedIndex}");
        }
    }

    // Method untuk membuka Panel Delete Special Seed ketika klik Button Jual
    // Digunakan pada OnClick setiap Button Delete Slot
    public void OpenPanelDeleteSpecialSeed()
    {
        panelDeleteSeed.SetActive(true);

        // Cek apakah slot punya seed (pastikan child lebih dari 0)
        if (specialSeedSlot.transform.childCount > 0)
        {
            // Child index 0 adalah seed
            GameObject seedObject = specialSeedSlot.transform.GetChild(0).gameObject; 

            string seedNameRaw = seedObject.name.Replace("(Clone)", "").Trim();

            SeedSpecialData seedData = seedConfig.GetSeedDataByPrefabName(seedNameRaw);

            if (seedData != null)
            {
                specialSeedName.text = $"'{seedData.seedName}'";
                currentDiscountedPrice = seedData.price / 2;
                specialSeedPrice.text = $"seharga '${currentDiscountedPrice}'";
                seedImageUI.sprite = seedData.seedImage;

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
    // Digunakan pada OnClick Button Iya di Panel Delete Special Seed
    public void ConfirmDeleteSpecialSeed()
    {
        RemoveSpecialSeedFromInventory();
        SaveSpecialSeeds();

        if (currentDiscountedPrice > 0)
        {
            refundCoin.text = $"+ {currentDiscountedPrice}";

            // Menambahkan Total Coin
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
        currentDiscountedPrice = 0; // Reset harga jual
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

    // Method untuk mengatur Particle System pada Special Seed di Inevntory
    // Digunakan pada Method DisplaySeedAndRefreshInfoCoroutine
    private void SetSeedParticleActive(GameObject seed, bool isActive)
    {
        if (seed == null) return;

        ParticleSystem particle = seed.GetComponentInChildren<ParticleSystem>(true); // true agar bisa cari yang awalnya nonaktif
        if (particle != null)
        {
            particle.gameObject.SetActive(isActive);
        }
    }
}