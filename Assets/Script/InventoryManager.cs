using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots; // Array slot UI untuk inventory
    private GameObject[] seedsInSlots; // Referensi biji yang ada di setiap slot

    void Start()
    {
        seedsInSlots = new GameObject[inventorySlots.Length]; // Inisialisasi array untuk menyimpan biji
    }

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

                return true;
            }
        }

        Debug.LogWarning("Inventory penuh!");
        return false; // Jika semua slot penuh
    }

    private void PlaceSeedInSlot(GameObject seed, GameObject slot)
    {
        // Set parent prefab biji menjadi slot inventory
        seed.transform.SetParent(slot.transform);

        // Reset posisi, rotasi, dan skala biji agar sesuai dengan slot
        seed.transform.localPosition = Vector3.zero;
        seed.transform.localRotation = Quaternion.identity;
        seed.transform.localScale = new Vector3(30f, 30f, 30f);

        Debug.Log("Biji dipindahkan ke slot: " + slot.name);
    }
}
