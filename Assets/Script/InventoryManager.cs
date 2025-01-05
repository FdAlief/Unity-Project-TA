using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots; // Array slot UI untuk inventory

    [SerializeField]
    private List<GameObject> seedsInSlots = new List<GameObject>(); // Database biji dalam list dinamis

    void Start()
    {
        // Set semua slot menjadi tidak aktif jika kosong prefab biji
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetActive(false);
        }
    }

    // Method untuk memasukkan biji ke dalam slot inventory yang ada
    // Digubakan pada script Congklak Hole (TransferSeedsToInventory)
    public bool AddSeedToInventory(GameObject seed)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Cek apakah slot kosong
            if (!inventorySlots[i].activeSelf)
            {
                // Tambahkan biji ke slot ini
                seedsInSlots.Add(seed); // Masukkan seed ke list
                inventorySlots[i].SetActive(true); // Aktifkan slot
                PlaceSeedInSlot(seed, inventorySlots[i]); // Pindahkan prefab biji ke slot
                StartCoroutine(TimeToChangeLayer(seed)); // Ubah Layer
                return true;
            }
        }

        Debug.LogWarning("Inventory penuh!");
        return false; // Jika semua slot penuh
    }

    // Method untuk menghapus data Seed pada Slot Inventory
    // Digunakan pada script DragHandler (HandleDrag - GetMouseUp/Touch Ended)
    public void RemoveSeedFromInventory(GameObject seed)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].activeSelf && seedsInSlots.Contains(seed))
            {
                seedsInSlots.Remove(seed); // Hapus seed dari list
                inventorySlots[i].SetActive(false); // Nonaktifkan slot
                ChangeSeedLayer(seed, "Default"); // Ubah layer kembali
                seed.transform.SetParent(null); // Lepaskan parent dari slot
                Debug.Log("Biji dihapus dari inventory: " + seed.name);
                break;
            }
        }
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

        Debug.Log("Biji dipindahkan ke slot: " + slot.name);
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

    // Method untuk mengubah layer prefab biji
    // Digunakan pada method AddSeedToInventory dan RemoveSeedFromInventory
    private void ChangeSeedLayer(GameObject seed, string layerName)
    {
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
}