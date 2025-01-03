using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHoleManager : MonoBehaviour
{
    public List<Collider> colliders; // List collider yang diatur secara berurutan

    void Start()
    {
        // Awalnya, aktifkan semua collider
        foreach (var col in colliders)
        {
            col.enabled = true;
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
}
