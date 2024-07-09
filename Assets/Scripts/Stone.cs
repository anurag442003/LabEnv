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

    // Reference to the agent managing the stone and its environment
    private MedicalVRAgent agent;

    /// <summary>
    /// Sets up the stone with its colliders and finds the agent reference
    /// </summary>
    private void Awake()
    {
        // Find stone and body colliders
        bodyCollider = transform.Find("BodyCollider").GetComponent<Collider>();
        stoneCollider = transform.Find("StoneCollider").GetComponent<Collider>();

        // Find the agent managing this stone
        agent = FindObjectOfType<MedicalVRAgent>();
        if (agent == null)
        {
            Debug.LogError("No MedicalVRAgent found in the scene!");
        }
    }

    /// <summary>
    /// Resets the stone's state
    /// </summary>
    public void ResetStone()
    {
        // Enable the stone and collider
        bodyCollider.gameObject.SetActive(true);
        stoneCollider.gameObject.SetActive(true);
        transform.localPosition = agent.InitialStonePosition;
    }

    /// <summary>
    /// Handles collision events with the stone
    /// </summary>
    /// <param name="other">The collider that entered into trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the stone collides with the target managed by the agent
        if (other.gameObject == agent.targetTransform.gameObject)
        {
            Debug.Log("Stone placed on the target.");
            agent.AddReward(1.0f); // Reward for placing the stone on the target
            agent.EndEpisode();
        }
    }
}
