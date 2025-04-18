using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderHoleManager : MonoBehaviour
{
    [Header("Hole Collider")]
    public List<Collider> colliders; // List collider yang diatur secara berurutan

    [Header("Info Non-Active Collider")]
    public List<Image> colliderImagesInfo; // List Image UI yang sesuai dengan collider
    public Sprite activeSprite; // Sprite untuk collider aktif
    public Sprite inactiveSprite; // Sprite untuk collider nonaktif

    [Header("Opposite Holes Mapping")]
    public Dictionary<int, int> oppositeHoles = new Dictionary<int, int>(); // Data untuk menentuka Hole yang Berlawanan

    void Start()
    {
        // Mapping hole berlawanan (1 -> 8, 2 -> 7, dll.)
        // Digunakan pada Script DragHandler (HandleDrag - MouseUp) dan SpecialSeedHandler (JamGadangSpecialSeed)
        oppositeHoles[1] = 9;
        oppositeHoles[2] = 8;
        oppositeHoles[3] = 7;
        oppositeHoles[4] = 6;
        oppositeHoles[9] = 1;
        oppositeHoles[8] = 2;
        oppositeHoles[7] = 3;
        oppositeHoles[6] = 4;

        // Nonaktifkan semua collider terlebih dahulu
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Aktifkan collider dengan indeks 0, 1, 2, dan 3 atau hole (1,2,3,4)
        for (int i = 0; i <= 3 && i < colliders.Count; i++)
        {
            colliders[i].enabled = true;
        }

        // Validasi panjang list Images
        if (colliders.Count != colliderImagesInfo.Count)
        {
            Debug.LogError("Jumlah colliders dan colliderImages tidak sesuai!");
            return;
        }

        // Perbarui UI berdasarkan status collider
        UpdateUICollider();
    }

    // Fungsi untuk ketika collider Hole dipilih maka collide yang aktif hanya collider setelahnya
    // Agar dalam peletkan biji ke Hole secara beurutan
    // Dipanggil pada script RaycastManager (HandleRaycastColliderHole) & (GetHoleUnderRaycast)
    public void OnColliderChoose(Collider clickedCollider)
    {
        // Cari indeks dari collider yang dipilih
        int clickedIndex = colliders.IndexOf(clickedCollider);

        // Jika collider yang dipilih ditemukan dalam daftar
        if (clickedIndex != -1)
        {
            Debug.Log($"Collider {clickedIndex} clicked!");

            // Nonaktifkan semua collider
            foreach (var col in colliders)
            {
                col.enabled = false;
            }

            // Hitung indeks berikutnya
            int nextIndex = (clickedIndex + 1) % colliders.Count;

            // Aktifkan collider berikutnya
            colliders[nextIndex].enabled = true;

            // Perbarui UI berdasarkan status collider
            UpdateUICollider();
        }
        else
        {
            Debug.LogWarning("Collider yang diklik tidak ditemukan dalam daftar!");
        }
    }

    // Fungsi untuk mengatur ulang collider ketika biji terakhir dilepas pada Hole Left
    // Method ini digunakan pada script DragHandler (MouseUp)
    public void ResetCollidersToDefault()
    {
        // Nonaktifkan semua collider terlebih dahulu
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Aktifkan collider dengan indeks 0, 1, 2, dan 3
        for (int i = 0; i <= 3 && i < colliders.Count; i++)
        {
            colliders[i].enabled = true;
        }

        // Perbarui UI berdasarkan status collider
        UpdateUICollider();
        Debug.Log("Collider di-reset ke default (0, 1, 2, 3 aktif)");
    }

    // Method ini berfungsi untuk menampilkan info UI Collider yang aktif dan nonaktif
    // Dengan merubah warna UI Infonya
    // Digunakan pada method (ResetCollidersToDefault), (OnColliderChoose), (Start) dan Script SpecialSeedHandler (HonaiSpecialSeed)
    public void UpdateUICollider()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].enabled)
            {
                colliderImagesInfo[i].sprite = activeSprite; // Ubah ke sprite aktif
            }
            else
            {
                colliderImagesInfo[i].sprite = inactiveSprite; // Ubah ke sprite nonaktif
            }
        }
    }
}
