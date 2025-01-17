using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class MedicalVRAgent : Agent
{
    [Header("Transforms")]
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    [Header("Input Actions")]
    public InputAction leftHandPositionAction;
    public InputAction leftHandRotationAction;
    public InputAction rightHandPositionAction;
    public InputAction rightHandRotationAction;

    [Header("Environment")]
    public StoneArea stoneArea;
    public Stone stone;

    private void OnEnable()
    {
        leftHandPositionAction.Enable();
        leftHandRotationAction.Enable();
        rightHandPositionAction.Enable();
        rightHandRotationAction.Enable();
    }

    private void OnDisable()
    {
        leftHandPositionAction.Disable();
        leftHandRotationAction.Disable();
        rightHandPositionAction.Disable();
        rightHandRotationAction.Disable();
    }

    public override void Initialize()
    {
        base.Initialize();
        if (stoneArea == null)
        {
            Debug.LogError("Stone Area not assigned!");
        }
        if (stone == null)
        {
            Debug.LogError("Stone not assigned!");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations for head and hands
        sensor.AddObservation(headTransform.localPosition);
        sensor.AddObservation(headTransform.localRotation);

        sensor.AddObservation(leftHandTransform.localPosition);
        sensor.AddObservation(leftHandTransform.localRotation);

        sensor.AddObservation(rightHandTransform.localPosition);
        sensor.AddObservation(rightHandTransform.localRotation);

        // Collect observation for the stone's position
        sensor.AddObservation(stone.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Apply actions to VR hands (if needed for your training logic)
        // Example: Control hand positions or triggers based on actions
        float leftHandX = actions.ContinuousActions[0];
        float leftHandY = actions.ContinuousActions[1];
        float leftHandZ = actions.ContinuousActions[2];
        float leftHandRotX = actions.ContinuousActions[3];
        float leftHandRotY = actions.ContinuousActions[4];
        float leftHandRotZ = actions.ContinuousActions[5];
        float leftHandRotW = actions.ContinuousActions[6];

        float rightHandX = actions.ContinuousActions[7];
        float rightHandY = actions.ContinuousActions[8];
        float rightHandZ = actions.ContinuousActions[9];
        float rightHandRotX = actions.ContinuousActions[10];
        float rightHandRotY = actions.ContinuousActions[11];
        float rightHandRotZ = actions.ContinuousActions[12];
        float rightHandRotW = actions.ContinuousActions[13];

        leftHandTransform.localPosition = new Vector3(leftHandX, leftHandY, leftHandZ);
        leftHandTransform.localRotation = new Quaternion(leftHandRotX, leftHandRotY, leftHandRotZ, leftHandRotW);

        rightHandTransform.localPosition = new Vector3(rightHandX, rightHandY, rightHandZ);
        rightHandTransform.localRotation = new Quaternion(rightHandRotX, rightHandRotY, rightHandRotZ, rightHandRotW);
        
        // Implement logic to detect and remove the stone
        if (IsStoneRemoved())
        {
            AddReward(1.0f); // Positive reward for removing the stone
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        // Use VR controller inputs as heuristic for training
        continuousActions[0] = leftHandPositionAction.ReadValue<Vector3>().x;
        continuousActions[1] = leftHandPositionAction.ReadValue<Vector3>().y;
        continuousActions[2] = leftHandPositionAction.ReadValue<Vector3>().z;

        continuousActions[3] = leftHandRotationAction.ReadValue<Quaternion>().x;
        continuousActions[4] = leftHandRotationAction.ReadValue<Quaternion>().y;
        continuousActions[5] = leftHandRotationAction.ReadValue<Quaternion>().z;
        continuousActions[6] = leftHandRotationAction.ReadValue<Quaternion>().w;

        continuousActions[7] = rightHandPositionAction.ReadValue<Vector3>().x;
        continuousActions[8] = rightHandPositionAction.ReadValue<Vector3>().y;
        continuousActions[9] = rightHandPositionAction.ReadValue<Vector3>().z;

        continuousActions[10] = rightHandRotationAction.ReadValue<Quaternion>().x;
        continuousActions[11] = rightHandRotationAction.ReadValue<Quaternion>().y;
        continuousActions[12] = rightHandRotationAction.ReadValue<Quaternion>().z;
        continuousActions[13] = rightHandRotationAction.ReadValue<Quaternion>().w;
    }

    private bool IsStoneRemoved()
    {
    // Define the logic to check if the stone has been removed
    // Example: Check the distance between the hand and the stone
        float distanceToStone = Vector3.Distance(leftHandTransform.position, stone.transform.position);
        if (distanceToStone < 0.1f) // Adjust the threshold as needed
        {
            // Remove all deformity from the stone
            float removedAmount = stone.RemoveDeformity(1f);
            
            // Check if the stone has been fully removed
            if (!stone.HasDeformity)
            {
                return true;
            }
        }
        return false;
    }
}
