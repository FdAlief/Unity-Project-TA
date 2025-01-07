using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHoleManager : MonoBehaviour
{
    public List<Collider> colliders; // List collider yang diatur secara berurutan

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

        Debug.Log("Collider di-reset ke default (0, 1, 2, 3 aktif)");
    }
}
