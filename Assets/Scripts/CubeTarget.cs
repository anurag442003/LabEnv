using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the stone is the object colliding with the cube
        if (other.CompareTag("Stone"))
        {
            Debug.Log("Stone has reached the cube.");
        }
    }
}