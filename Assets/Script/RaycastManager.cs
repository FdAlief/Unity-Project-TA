using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public LayerMask holeLayer; // Layer untuk mendeteksi lubang congklak

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Klik kiri mouse (atau tap di Android)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast hanya mendeteksi objek pada layer tertentu
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, holeLayer))
            {
                // Cek apakah objek memiliki script CongklakHole
                CongklakHole hole = hit.collider.GetComponent<CongklakHole>();
                if (hole != null)
                {
                    hole.HandleClick(); // Panggil logika lubang saat diklik
                    Debug.Log("Lubang diklik: " + hit.collider.gameObject.name);
                }
            }
        }
    }
}
