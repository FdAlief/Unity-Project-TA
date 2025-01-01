using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots; // Array slot UI untuk inventory

    [SerializeField]
    private GameObject[] seedsInSlots; // Database biji yang ada di setiap slot

    void Start()
    {
        seedsInSlots = new GameObject[inventorySlots.Length]; // Inisialisasi array untuk menyimpan biji
    }

    // Method untuk memasukkan biji ke dalam slot inventory yang ada
    // Digubakan pada script Congklak Hole (TransferSeedsToInventory)
    public bool AddSeedToInventory(GameObject seed)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Cek apakah slot kosong
            if (seedsInSlots[i] == null)
            {
                // Tambahkan biji ke slot ini
                seedsInSlots[i] = seed;

                // Pindahkan prefab biji ke dalam slot
                PlaceSeedInSlot(seed, inventorySlots[i]);

                // Ubah Layer menggunakan waktu
                StartCoroutine(TimeToChangeLayer(seed));

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
        for (int i = 0; i < seedsInSlots.Length; i++)
        {
            if (seedsInSlots[i] == seed)
            {
                seedsInSlots[i] = null; // Hapus dari slot

                // Ubah layer (di luar inventory)
                ChangeSeedLayer(seed, "Default");

                break;
            }
        }

        // Pastikan biji tidak lagi menjadi child dari slot
        seed.transform.SetParent(null);
        Debug.Log("Biji dihapus dari inventory: " + seed.name);
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
        // Periksa apakah biji ada di salah satu slot inventory
        foreach (var s in seedsInSlots)
        {
            if (s == seed)
            {
                return true;
            }
        }
        return false;
    }

    // Method untuk mengecek apakah inventory kosong
    // Digunakan pada script CongklakHole (HandleClick)
    // Fungsi agar membatasi mengambil biji dari hole ke Inventory
    // Harus kosong slot inventory baru bisa ambil biji dari Hole
    public bool IsInventoryEmpty()
    {
        foreach (var seed in seedsInSlots)
        {
            if (seed != null) // Jika ada slot yang terisi
            {
                return false;
            }
        }
        return true; // Semua slot kosong
    }

    // Method untuk mengubah layer prefab biji
    // Digunakan pada method AddSeedToInventory dan RemoveSeedFromInventory
    private void ChangeSeedLayer(GameObject seed, string layerName)
    {
        // Ubah layer pada seed
        seed.layer = LayerMask.NameToLayer(layerName);
        Debug.Log("Layer biji diubah menjadi: " + layerName);
    }

    IEnumerator TimeToChangeLayer(GameObject seed)
    {
        yield return new WaitForSeconds(0.05f); // Menunggu 0.05 detik
        ChangeSeedLayer(seed, "Seed Layer"); // Ubah Layer
    }
}