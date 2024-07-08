using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a single stone on the body
/// </summary>
public class Stone : MonoBehaviour
{
    // The trigger collider representing the stone
    [HideInInspector]
    public Collider stoneCollider;

    // The solid collider representing the stone
    private Collider bodyCollider;

    /// <summary>
    /// Resets the stone
    /// </summary>
    public void ResetStone()
    {
        // Enable the stone and collider
        bodyCollider.gameObject.SetActive(true);
        stoneCollider.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the stone wakes up
    /// </summary>
    private void Awake()
    {
        // Find stone and body colliders
        bodyCollider = transform.Find("BodyCollider").GetComponent<Collider>();
        stoneCollider = transform.Find("StoneCollider").GetComponent<Collider>();
    }
}
