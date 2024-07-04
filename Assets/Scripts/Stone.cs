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

    // The stone's material
    private Material stoneMaterial;

    /// <summary>
    /// The amount of deformity in the stone
    /// </summary>
    public float DeformityAmount { get; private set; }

    /// <summary>
    /// Whether the stone has any deformity remaining
    /// </summary>
    public bool HasDeformity
    {
        get
        {
            return DeformityAmount > 0f;
        }
    }

    /// <summary>
    /// Attempts to remove the deformity from the stone
    /// </summary>
    /// <param name="amount">The amount of deformity to remove</param>
    /// <returns>The actual amount successfully removed</returns>
    public float RemoveDeformity(float amount)
    {
        // Track how much deformity was successfully removed
        float deformityRemoved = Mathf.Clamp(amount, 0f, DeformityAmount);

        // Subtract the deformity
        DeformityAmount -= amount;

        if (DeformityAmount <= 0)
        {
            // No deformity remaining
            DeformityAmount = 0;

            // Disable the stone and collider
            bodyCollider.gameObject.SetActive(false);
            stoneCollider.gameObject.SetActive(false);

            // Change the stone color to indicate it is removed
            stoneMaterial.SetColor("_BaseColor", Color.clear);
        }

        // Return the amount of deformity that was removed
        return deformityRemoved;
    }

    /// <summary>
    /// Resets the stone
    /// </summary>
    public void ResetStone()
    {
        // Refill the deformity
        DeformityAmount = 1f;

        // Enable the stone and collider
        bodyCollider.gameObject.SetActive(true);
        stoneCollider.gameObject.SetActive(true);

        // Change the stone color to indicate it is full
        stoneMaterial.SetColor("_BaseColor", Color.red);
    }

    /// <summary>
    /// Called when the stone wakes up
    /// </summary>
    private void Awake()
    {
        // Find the stone's mesh renderer and get the main material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        stoneMaterial = meshRenderer.material;

        // Find stone and body colliders
        bodyCollider = transform.Find("BodyCollider").GetComponent<Collider>();
        stoneCollider = transform.Find("StoneCollider").GetComponent<Collider>();
    }
}
