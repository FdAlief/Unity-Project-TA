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
    public Color activeColor = Color.green; // Warna untuk collider aktif
    public Color inactiveColor = Color.red; // Warna untuk collider nonaktif

    void Start()
    {
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
    // Digunakan pada method (ResetCollidersToDefault), (OnColliderChoose), (Start)
    private void UpdateUICollider()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].enabled)
            {
                colliderImagesInfo[i].color = activeColor; // Ubah ke warna aktif
            }
            else
            {
                colliderImagesInfo[i].color = inactiveColor; // Ubah ke warna nonaktif
            }
        }
    }
}
