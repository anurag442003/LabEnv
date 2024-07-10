using UnityEngine;

public class Stone : MonoBehaviour
{
    public Collider triggerCollider;
    private Rigidbody rb;
    private MedicalVRAgent agent;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;  // Ensure it's not kinematic
        }
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Find the MedicalVRAgent in the scene
        agent = FindObjectOfType<MedicalVRAgent>();
        if (agent == null)
        {
            Debug.LogError("MedicalVRAgent not found in the scene!");
        }
    }

    public void ResetStone()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        Debug.Log("Stone reset to initial position.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (agent != null && agent.targetTransform != null && other.gameObject == agent.targetTransform.gameObject)
        {
            Debug.Log($"Stone placed on target in episode {agent.GetCurrentEpisode()}. Attempting to record demonstration.");
            agent.AddReward(1.0f);
            agent.EndEpisode();
        }
    }
}